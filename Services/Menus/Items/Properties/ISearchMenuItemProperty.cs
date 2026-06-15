using Entities.Menus;

namespace Services.Menus.Items.Properties;

public interface ISearchMenuItemProperty
{
    IQueryable<MenuItemProperty> Respond(Request request);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        public int? MenuItemId { get; set; }
        public ISearchMenuItems.Request MenuItem { get; set; }
        public string Title { get; set; }
    }
}