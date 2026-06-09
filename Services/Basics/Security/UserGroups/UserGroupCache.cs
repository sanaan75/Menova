using Entities.Basics.Security;
using Entities.Caches;

namespace Services.Basics.Security.UserGroups;

public class UserGroupCache : IUserGroupCache
{
    private readonly IContainer _mainContainer;
    private readonly IFullCache<int, IUserGroupCache.Model> _cache;

    public UserGroupCache(IContainer mainContainer, IFullCache<int, IUserGroupCache.Model> cache)
    {
        _mainContainer = mainContainer;
        _cache = cache;
        _cache.Fill += Fill;
    }

    private Dictionary<int, IUserGroupCache.Model> Fill()
    {
        using var container = _mainContainer.Get<IContainer>();
        var db = container.Get<IDb>();

        return db.Query<UserGroup>().Select(i => new IUserGroupCache.Model
            {
                Id = i.Id,
                Title = i.Title
            })
            .ToList()
            .ToDictionary(i => i.Id, i => i);
    }

    public void Reset()
    {
        _cache.Reset();
    }

    public IUserGroupCache.Model Get(int? id)
    {
        return _cache.Get(id);
    }

    public IList<IUserGroupCache.Model> GetAll()
    {
        return _cache.GetValues();
    }
}