using Entities.Caches;
using Entities.Plans;

namespace Services.Plans;

public class PlanCache : IPlanCache
{
    private readonly IContainer _mainContainer;
    private readonly IFullCache<int, IPlanCache.Model> _cache;

    public PlanCache(IContainer mainContainer, IFullCache<int, IPlanCache.Model> cache)
    {
        _mainContainer = mainContainer;
        _cache = cache;
        _cache.Fill += Fill;
    }

    private Dictionary<int, IPlanCache.Model> Fill()
    {
        using var container = _mainContainer.Get<IContainer>();
        var db = container.Get<IDb>();

        return db.Query<Plan>().Select(i => new IPlanCache.Model
            {
                Id = i.Id,
                Name = i.Name,
                MaxItems = i.MaxItems,
                MaxImagePerItem = i.MaxImagePerItem,
                Price=i.Price,
                IsActive = i.IsActive
            })
            .ToList()
            .ToDictionary(i => i.Id, i => i);
    }

    public void Reset()
    {
        _cache.Reset();
    }

    public IPlanCache.Model Get(int? id)
    {
        return _cache.Get(id);
    }

    public IList<IPlanCache.Model> GetAll()
    {
        return _cache.GetValues();
    }
}