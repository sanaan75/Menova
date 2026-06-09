using Entities;
using Entities.Plans;

namespace Services.Plans;

public class SearchPlans(IDb db) : ISearchPlans
{
    public IQueryable<Plan> Respond(ISearchPlans.Request request = null)
    {
        var items = db.Query<Plan>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.Name, i => i.Name == request.Name);


        if (request.IsActive is not null)
            items = items.Where(i => i.IsActive == request.IsActive.Value);

        return items.OrderByDescending(i => i.Id);
    }
}