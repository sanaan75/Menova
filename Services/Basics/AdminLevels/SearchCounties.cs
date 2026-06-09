using Entities;
using Entities.Basics.AdminLevels;

namespace Services.Basics.AdminLevels;

public class SearchCounties(IDb db) : ISearchCounties
{
    public IQueryable<County> Respond(ISearchCounties.Request request)
    {
        var items = db.Query<County>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.Filter(request.ProvinceId, i => i.ProvinceId == request.ProvinceId.Value);

        if (string.IsNullOrWhiteSpace(request.Name) == false)
            items = items.Where(i => i.Name.Contains(request.Name));

        return items.OrderByDescending(i => i.Id);
    }
}