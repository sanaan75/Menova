using Entities;
using Entities.Menus;

namespace Services.Menus.Items.Properties;

public class SearchMenuItemProperty(IDb db) : ISearchMenuItemProperty
{
    public IQueryable<MenuItemProperty> Respond(ISearchMenuItemProperty.Request request)
    {
        var items = db.Query<MenuItemProperty>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.MenuItemId, i => i.MenuItemId == request.MenuItemId);
        items = items.Filter(request.Title, i => i.Title == request.Title);

        return items.OrderByDescending(i => i.Id);
    }
}