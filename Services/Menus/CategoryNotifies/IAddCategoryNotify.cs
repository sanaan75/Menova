using Entities.Menus;

namespace Services.Menus.CategoryNotifies;

public interface IAddCategoryNotify
{
    CategoryNotify Respond(Request request);

    class Request
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}