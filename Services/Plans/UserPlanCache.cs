using Entities.Caches;
using Entities.Plans;

namespace Services.Plans;

public class UserPlanCache : IUserPlanCache
{
    private readonly IContainer _mainContainer;
    private readonly IFullCache<int, IUserPlanCache.Model> _cache;

    public UserPlanCache(IContainer mainContainer, IFullCache<int, IUserPlanCache.Model> cache)
    {
        _mainContainer = mainContainer;
        _cache = cache;
        _cache.Fill += Fill;
    }

    private Dictionary<int, IUserPlanCache.Model> Fill()
    {
        using var container = _mainContainer.Get<IContainer>();
        var db = container.Get<IDb>();

        return db.Query<UserPlan>().Select(i => new IUserPlanCache.Model
            {
                Id = i.Id,
                UserId = i.UserId,
                PlanId = i.PlanId,
                StartDate = i.StartDate,
                EndDate = i.EndDate,
            })
            .ToList()
            .ToDictionary(i => i.Id, i => i);
    }

    public void Reset()
    {
        _cache.Reset();
    }

    public IUserPlanCache.Model Get(int? id)
    {
        return _cache.Get(id);
    }

    public IList<IUserPlanCache.Model> GetAll()
    {
        return _cache.GetValues();
    }

    public IList<IUserPlanCache.Model> GetByUser(int userId)
    {
        return GetAll().Where(i => i.UserId == userId).ToList();
    }
}