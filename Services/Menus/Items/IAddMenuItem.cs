using Entities.Menus;

namespace Services.Menus.Items;

public interface IAddMenuItem
{
    MenuItem Respond(Request request);

    class Request
    {
        public int SubCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Order { get; set; }
    }
}