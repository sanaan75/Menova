using Entities.Menus;

namespace Services.Menus.Categories;

public interface ISearchCategories
{
    IQueryable<Category> Respond(Request request);

    class Request
    {
        public int? Id { get; set; }
        public IList<int> Ids { get; set; }
        public int? IgnoredId { get; set; }
        public int? MenuId { get; set; }
        public ISearchMenus.Request Menu { get; set; }
        public string Name { get; set; }public bool? IsActive { get; set; }}
}