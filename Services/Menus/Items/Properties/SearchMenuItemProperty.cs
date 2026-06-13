using Entities;
using Entities.Menus;

namespace Services.Menus.Items.Properties;

public class SearchMenuItemProperty(IDb db, ISearchMenuItems searchMenuItems) : ISearchMenuItemProperty
{
    public IQueryable<MenuItemProperty> Respond(ISearchMenuItemProperty.Request request)
    {
        var items = db.Query<MenuItemProperty>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.MenuItemId, i => i.MenuItemId == request.MenuItemId);
        
        if (request.MenuItem is not null)
            items = from i in items join j in searchMenuItems.Respond(request.MenuItem) on i.MenuItemId equals j.Id select i;
        
        items = items.Filter(request.Title, i => i.Title == request.Title);

        return items.OrderByDescending(i => i.Id);
    }
}