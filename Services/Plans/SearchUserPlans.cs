using Entities;
using Entities.Plans;

namespace Services.Plans;

public class SearchUserPlans(IDb db) : ISearchUserPlans
{
    public IQueryable<UserPlan> Respond(ISearchUserPlans.Request request = null)
    {
        var items = db.Query<UserPlan>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);
        
        items = items.Filter(request.UserId, i => i.UserId == request.UserId.Value);
        items = items.Filter(request.PlanId, i => i.PlanId == request.PlanId.Value);

        return items.OrderByDescending(i => i.Id);
    }
}