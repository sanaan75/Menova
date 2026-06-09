using Entities.Menus;

namespace Services.Menus.SubCategoryNotifies;

public interface ISearchSubCategoryNotifies
{
    IQueryable<SubCategoryNotify> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        public int? SubCategoryId { get; set; }
        public string Title { get; set; }
        public bool? IsActive { get; set; }
    }
}