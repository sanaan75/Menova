using Entities.Menus;

namespace Services.Menus;

public interface ISearchMenus
{
    IQueryable<Menu> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
    }
}