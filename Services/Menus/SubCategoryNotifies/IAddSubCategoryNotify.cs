using Entities.Menus;

namespace Services.Menus.SubCategoryNotifies;

public interface IAddSubCategoryNotify
{
    SubCategoryNotify Respond(Request request);

    class Request
    {
        public int UserId { get; set; }
        public int SubCategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}