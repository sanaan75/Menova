using Entities.Menus;

namespace Services.Menus.Items.Properties;

public interface IAddMenuItemProperty
{
    MenuItemProperty Respond(Request request = null);

    class Request
    {
        public int MenuItemId { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
    }
}