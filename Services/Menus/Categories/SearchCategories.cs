using Entities;
using Entities.Menus;

namespace Services.Menus.Categories;

public class SearchCategories(IDb db, ISearchMenus searchMenus) : ISearchCategories
{
    public IQueryable<Category> Respond(ISearchCategories.Request request)
    {
        var items = db.Query<Category>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.MenuId, i => i.MenuId == request.MenuId.Value);

        if (request.Menu is not null)
            items = from i in items join j in searchMenus.Respond(request.Menu) on i.MenuId equals j.Id select i;

        items = items.Filter(request.Name, i => i.Name == request.Name);
        items = items.Filter(request.IsActive, i => i.IsActive == request.IsActive);

        return items.OrderByDescending(i => i.Id);
    }
}