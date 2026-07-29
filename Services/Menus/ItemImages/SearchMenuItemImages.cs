using Entities;
using Entities.Menus;
using Services.Menus.Items;

namespace Services.Menus.ItemImages;

public class SearchMenuItemImages(IDb db, ISearchMenuItems searchMenuItems) : ISearchMenuItemImages
{
    public IQueryable<MenuItemImage> Respond(ISearchMenuItemImages.Request request)
    {
        var items = db.Query<MenuItemImage>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.MenuItemId, i => i.MenuItemId == request.MenuItemId!.Value);

        if (request.MenuItem is not null)
            items = from i in items join j in searchMenuItems.Respond(request.MenuItem) on i.MenuItemId equals j.Id select i;

        return items.OrderByDescending(i => i.Id);
    }
}