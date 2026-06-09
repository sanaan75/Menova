using Entities.Menus;

namespace Services.Menus.SubCategories;

public interface IAddSubCategory
{
    SubCategory Respond(Request request);

    class Request
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}