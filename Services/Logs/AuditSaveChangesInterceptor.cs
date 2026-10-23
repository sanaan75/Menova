using Entities;
using Entities.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Services.Settings;

namespace Services.Logs;

public class AuditSaveChangesInterceptor(
    IObjectChangeComparer comparer,
    IActorService actorService,
    ITimeService timeService,
    ISettingsCache settingsCache)
    : SaveChangesInterceptor
{
    private readonly List<PendingAuditEntry> _pendingAuditEntries = [];
    private bool _isSavingAuditLogs;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        PrepareAuditEntries(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        PrepareAuditEntries(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        SaveAuditLogs(eventData.Context);

        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        await SaveAuditLogsAsync(eventData.Context, cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _pendingAuditEntries.Clear();

        base.SaveChangesFailed(eventData);
    }

    public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        _pendingAuditEntries.Clear();

        await base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private void PrepareAuditEntries(DbContext? context)
    {
        if (context == null || _isSavingAuditLogs)
            return;

        var entries = context.ChangeTracker
            .Entries()
            .Where(i =>
                i.State == EntityState.Added ||
                i.State == EntityState.Modified ||
                i.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            if (entry.Entity is not ActionLog actionLogEntity || entry.Entity is not IEntity entity)
                continue;

            _pendingAuditEntries.Add(new PendingAuditEntry
            {
                Entity = entity,
                EntityName = entry.Metadata.GetTableName() ?? entry.Metadata.ClrType.Name,
                ModuleName = actionLogEntity.ModuleEntityProperty,
                State = entry.State,
                OriginalValues = entry.State == EntityState.Added
                    ? null
                    : entry.OriginalValues.ToObject(),
                CurrentValues = entry.State == EntityState.Deleted
                    ? null
                    : entry.CurrentValues.ToObject()
            });
        }
    }

    private void SaveAuditLogs(DbContext? context)
    {
        if (context == null || _isSavingAuditLogs || _pendingAuditEntries.Count == 0)
            return;

        var logs = BuildAuditLogs();
        if (logs.Count == 0)
            return;

        try
        {
            _isSavingAuditLogs = true;
            using var transaction = context.Database.CurrentTransaction == null
                ? context.Database.BeginTransaction()
                : null;
            RemoveOldLogsForNewLogs(context, logs.Count);
            context.Set<ActionLogEntry>().AddRange(logs);
            context.SaveChanges();
            transaction?.Commit();
        }
        finally
        {
            _isSavingAuditLogs = false;
        }
    }

    private async Task SaveAuditLogsAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context == null || _isSavingAuditLogs || _pendingAuditEntries.Count == 0)
            return;

        var logs = BuildAuditLogs();
        if (logs.Count == 0)
            return;

        try
        {
            _isSavingAuditLogs = true;
            await using var transaction = context.Database.CurrentTransaction == null
                ? await context.Database.BeginTransactionAsync(cancellationToken)
                : null;
            await RemoveOldLogsForNewLogsAsync(context, logs.Count, cancellationToken);
            context.Set<ActionLogEntry>().AddRange(logs);
            await context.SaveChangesAsync(cancellationToken);
            if (transaction != null)
                await transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            _isSavingAuditLogs = false;
        }
    }

    private List<ActionLogEntry> BuildAuditLogs()
    {
        var pendingAuditEntries = _pendingAuditEntries.ToList();
        _pendingAuditEntries.Clear();

        var now = timeService.Now;
        var userId = actorService.UserId.ToString();
        var result = new List<ActionLogEntry>();

        foreach (var entry in pendingAuditEntries)
        {
            var logData = GetLogData(entry);
            if (entry.State == EntityState.Modified && string.IsNullOrEmpty(logData))
                continue;

            result.Add(new ActionLogEntry
            {
                ModuleName = entry.ModuleName,
                ModuleEntityId = entry.Entity.Id,
                EntityName = entry.EntityName,
                EntityId = entry.Entity.Id,
                ActionName = GetActionName(entry.State),
                ActionDate = now,
                UserId = userId,
                LogData = logData
            });
        }

        return result;
    }

    private void RemoveOldLogsForNewLogs(DbContext context, int newLogCount)
    {
        var maxCount = settingsCache.Get().ActionLogMaxCount;
        if (maxCount <= 0)
            return;

        var countToDelete = context.Set<ActionLogEntry>().Count() + newLogCount - maxCount;
        if (countToDelete <= 0)
            return;

        context.Set<ActionLogEntry>()
            .OrderBy(i => i.ActionDate)
            .ThenBy(i => i.Id)
            .Take(countToDelete)
            .ExecuteDelete();
    }

    private async Task RemoveOldLogsForNewLogsAsync(DbContext context, int newLogCount, CancellationToken cancellationToken)
    {
        var maxCount = settingsCache.Get().ActionLogMaxCount;
        if (maxCount <= 0)
            return;

        var countToDelete = await context.Set<ActionLogEntry>().CountAsync(cancellationToken) + newLogCount - maxCount;
        if (countToDelete <= 0)
            return;

        await context.Set<ActionLogEntry>()
            .OrderBy(i => i.ActionDate)
            .ThenBy(i => i.Id)
            .Take(countToDelete)
            .ExecuteDeleteAsync(cancellationToken);
    }

    private string GetLogData(PendingAuditEntry entry)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                comparer.Respond(entry.CurrentValues!, string.Empty);
                break;

            case EntityState.Modified:
                comparer.Respond(entry.OriginalValues!, entry.CurrentValues!, string.Empty);
                break;

            case EntityState.Deleted:
                comparer.Respond(entry.OriginalValues!, string.Empty);
                break;
        }

        return comparer.GetChangesTextWithOutAndReset();
    }

    private static string GetActionName(EntityState state)
    {
        return state switch
        {
            EntityState.Added => Glossary.Add,
            EntityState.Modified => Glossary.Edit,
            EntityState.Deleted => Glossary.Delete,
            _ => state.ToString()
        };
    }

    private class PendingAuditEntry
    {
        public IEntity Entity { get; init; }
        public string EntityName { get; init; }
        public LogModule ModuleName { get; init; }
        public EntityState State { get; init; }
        public object? OriginalValues { get; init; }
        public object? CurrentValues { get; init; }
    }
}
