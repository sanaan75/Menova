using Entities.Menus;
using Services.Menus.SubCategories;

namespace Services.Menus.ItemImages;

public interface IAddMenuItemImage
{
    MenuItemImage Respond(Request request);

    class Request
    {
        public ISearchSubCategories.Request MenuSubCategories { get; set; }
        public int MenuItemId { get; set; }
        public string ImageUrl { get; set; }
    }
}