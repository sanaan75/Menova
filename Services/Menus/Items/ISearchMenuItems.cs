using Entities.Menus;
using Services.Menus.SubCategories;

namespace Services.Menus.Items;

public interface ISearchMenuItems
{
    IQueryable<MenuItem> Respond(Request request);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        public int? SubCategoryId { get; set; }
        public ISearchSubCategories.Request SubCategory { get; set; }
        public string Name { get; set; }public bool? IsActive { get; set; }
    }
}