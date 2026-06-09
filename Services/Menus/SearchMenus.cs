using Entities;
using Entities.Menus;

namespace Services.Menus;

public class SearchMenus(IDb db) : ISearchMenus
{
    public IQueryable<Menu> Respond(ISearchMenus.Request request)
    {
        var items = db.Query<Menu>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);


        items = items.Filter(request.UserId, i => i.UserId == request.UserId);
        items = items.Filter(request.Name, i => i.Name == request.Name);

        return items.OrderByDescending(i => i.Id);
    }
}