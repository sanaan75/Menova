using Entities.Logs;
using Microsoft.EntityFrameworkCore;
using Services.Settings;

namespace Services.Logs;

public class CleanupActionLogs(IDb db, ISettingsCache settingsCache, ITimeService timeService) : ICleanupActionLogs
{
    private const int DeleteBatchSize = 1_000;

    public async Task<ICleanupActionLogs.Response> Respond(CancellationToken cancellationToken = default)
    {
        var settings = settingsCache.Get();
        if (settings.ActionLogMaxDays <= 0)
            return new ICleanupActionLogs.Response { IsSkipped = true };

        var result = new ICleanupActionLogs.Response();
        var cutoff = timeService.Now.AddDays(-settings.ActionLogMaxDays);

        result.DeletedByAge = await DeleteOlderThanAsync(cutoff, cancellationToken);

        return result;
    }

    private async Task<int> DeleteOlderThanAsync(DateTime cutoff, CancellationToken cancellationToken)
    {
        var deletedCount = 0;

        while (true)
        {
            var entries = await db.Set<ActionLogEntry>()
                .Where(i => i.ActionDate < cutoff)
                .OrderBy(i => i.ActionDate)
                .ThenBy(i => i.Id)
                .Take(DeleteBatchSize)
                .ToListAsync(cancellationToken);

            if (entries.Count == 0)
                return deletedCount;

            db.Set<ActionLogEntry>().RemoveRange(entries);
            await db.SaveAsync();
            deletedCount += entries.Count;
        }
    }

}
