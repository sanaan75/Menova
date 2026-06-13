using Entities.Menus;
using Entities.Users;

namespace Services.Menus;

public interface IAddMenu
{
    Menu Respond(Request request);

    class Request
    {
        public int? UserId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}