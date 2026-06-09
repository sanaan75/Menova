using Entities.Menus;
using Services.Menus.Items;

namespace Services.Menus.ItemImages;

public interface ISearchMenuItemImages
{
    IQueryable<MenuItemImage> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        public int? MenuItemId { get; set; }
        public ISearchMenuItems.Request MenuItem { get; set; }
    }
}