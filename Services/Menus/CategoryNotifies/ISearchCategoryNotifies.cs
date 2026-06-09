using Entities.Menus;

namespace Services.Menus.CategoryNotifies;

public interface ISearchCategoryNotifies
{
    IQueryable<CategoryNotify> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; }
        public bool? IsActive { get; set; }
    }
}