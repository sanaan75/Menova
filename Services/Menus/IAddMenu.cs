using Entities.Menus;

namespace Services.Menus;

public interface IAddMenu
{
    Menu Respond(Request request);

    class Request
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}