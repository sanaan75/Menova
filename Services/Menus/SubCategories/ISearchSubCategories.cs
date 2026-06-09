using Entities.Menus;
using Services.Menus.Categories;

namespace Services.Menus.SubCategories;

public interface ISearchSubCategories
{
    IQueryable<SubCategory> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        
        public int? CategoryId { get; set; }
        public ISearchCategories.Request Category { get; set; }
        
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}