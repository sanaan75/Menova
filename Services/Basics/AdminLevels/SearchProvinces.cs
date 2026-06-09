using Entities;
using Entities.Basics.AdminLevels;

namespace Services.Basics.AdminLevels;

public class SearchProvinces(IDb db) : ISearchProvinces
{
    public IQueryable<Province> Respond(ISearchProvinces.Request request)
    {
        var items = db.Query<Province>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);

        if (string.IsNullOrWhiteSpace(request.Name) == false)
            items = items.Where(i => i.Name.Contains(request.Name));

        return items.OrderByDescending(i => i.Id);
    }
}