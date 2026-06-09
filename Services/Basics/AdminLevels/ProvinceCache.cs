using Entities.Basics.AdminLevels;
using Entities.Caches;

namespace Services.Basics.AdminLevels;

public class ProvinceCache : IProvinceCache
{
    private readonly IContainer _mainContainer;
    private readonly IFullCache<int, IProvinceCache.Model> _cache;

    public ProvinceCache(IContainer mainContainer, IFullCache<int, IProvinceCache.Model> cache)
    {
        _mainContainer = mainContainer;
        _cache = cache;
        _cache.Fill += Fill;
    }

    private Dictionary<int, IProvinceCache.Model> Fill()
    {
        using var container = _mainContainer.Get<IContainer>();
        var db = container.Get<IDb>();

        return db.Query<Province>().Select(i => new IProvinceCache.Model
            {
                Id = i.Id,
                Name = i.Name
            })
            .ToList()
            .ToDictionary(i => i.Id, i => i);
    }

    public void Reset()
    {
        _cache.Reset();
    }

    public IProvinceCache.Model Get(int? id)
    {
        return _cache.Get(id);
    }

    public IList<IProvinceCache.Model> GetAll()
    {
        return _cache.GetValues();
    }
}