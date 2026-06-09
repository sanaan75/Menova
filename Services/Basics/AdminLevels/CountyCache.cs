using Entities.Basics.AdminLevels;
using Entities.Caches;

namespace Services.Basics.AdminLevels;

public class CountyCache : ICountyCache
{
    private readonly IContainer _mainContainer;
    private readonly IFullCache<int, ICountyCache.Model> _cache;

    public CountyCache(IContainer mainContainer, IFullCache<int, ICountyCache.Model> cache)
    {
        _mainContainer = mainContainer;
        _cache = cache;
        _cache.Fill += Fill;
    }

    private Dictionary<int, ICountyCache.Model> Fill()
    {
        using var container = _mainContainer.Get<IContainer>();
        var db = container.Get<IDb>();

        return db.Query<County>().Select(i => new ICountyCache.Model
            {
                Id = i.Id,
                Name = i.Name,
                ProvinceId = i.ProvinceId,
            })
            .ToList()
            .ToDictionary(i => i.Id, i => i);
    }

    public void Reset()
    {
        _cache.Reset();
    }

    public ICountyCache.Model Get(int? id)
    {
        return _cache.Get(id);
    }

    public IList<ICountyCache.Model> GetAll()
    {
        return _cache.GetValues();
    }

    public IList<ICountyCache.Model> GetProvinceCounties(int id)
    {
        return _cache.GetValues().Where(i=>i.ProvinceId==id).ToList();
    }
}