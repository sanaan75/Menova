using Entities.Menus;
using Services.Users;

namespace Services.Menus;

public interface ISearchMenus
{
    IQueryable<Menu> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        public int? UserId { get; set; }
        public ISearchUsers.Request User { get; set; }
        public string Name { get; set; }
    }
}