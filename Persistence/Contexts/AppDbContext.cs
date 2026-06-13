using Entities;
using Entities.Basics.AdminLevels;
using Entities.Basics.Security;
using Entities.Menus;
using Entities.Security;
using Entities.Settings;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Persistence.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IDb
{
    private readonly IList<Action> _postSaveActions = new List<Action>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(_PersistenceDummy).Assembly);

        SetConventions(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void SetConventions(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }
    }

    public virtual bool AutoDetectChanges
    {
        get => ChangeTracker.AutoDetectChangesEnabled;
        set => ChangeTracker.AutoDetectChangesEnabled = value;
    }

    public int? CommandTimeOut
    {
        set => Database.SetCommandTimeout(value);
    }

    public void AddPostSaveAction(Action action) => _postSaveActions.Add(action);

    public DbSet<Setting> Settings { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<UserGroupPermission> UserGroupPermissions { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<UserConfirmCode> UserConfirmCodes { get; set; }

    public DbSet<Province> Provinces { get; set; }
    public DbSet<County> Counties { get; set; }

    public DbSet<AppSession> AppSessions { get; set; }
    public DbSet<TokenDetail> TokenDetails { get; set; }
    // public DbSet<SystemLog> SystemLogs { get; set; }

    public DbSet<Menu> Menus { get; set; }

    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryNotify> CategoryNotifies { get; set; }

    public DbSet<SubCategory> SubCategories { get; set; }
    public DbSet<SubCategoryNotify> SubCategoryNotifies { get; set; }

    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<MenuItemImage> MenuItemImages { get; set; }
    public DbSet<MenuItemProperty> MenuItemProperties { get; set; }

    public IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity
    {
        return Set<TEntity>().AsNoTracking().AsQueryable().OrderByDescending(i => i.Id);
    }

    public void RemoveById<TEntity>(int id) where TEntity : class, IEntity, new()
    {
        var entity = NewTrackedEntity<TEntity>(id);
        Set<TEntity>().Remove(entity);
    }

    public void RemoveByIds<TEntity>(IEnumerable<int> ids) where TEntity : class, IEntity, new()
    {
        var entities = ids.Select(NewTrackedEntity<TEntity>).ToList();
        Set<TEntity>().RemoveRange(entities);
    }

    private TEntity NewTrackedEntity<TEntity>(int id) where TEntity : class, IEntity, new()
    {
        var entity = new TEntity { Id = id };
        Attach(entity);
        Entry(entity).State = EntityState.Unchanged;

        return entity;
    }

    public void Save()
    {
        base.SaveChanges();

        foreach (var action in _postSaveActions)
            action.Invoke();

        _postSaveActions.Clear();
    }

    public async Task SaveAsync()
    {
        await base.SaveChangesAsync();

        foreach (var action in _postSaveActions)
            action.Invoke();

        _postSaveActions.Clear();
    }
}