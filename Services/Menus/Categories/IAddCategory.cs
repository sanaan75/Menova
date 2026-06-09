using Entities.Menus;

namespace Services.Menus.Categories;

public interface IAddCategory
{
    Category Respond(Request request);

    class Request
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}