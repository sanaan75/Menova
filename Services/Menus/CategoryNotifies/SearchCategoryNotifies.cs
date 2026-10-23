using Entities;
using Entities.Menus;

using Services.Menus.Categories;

namespace Services.Menus.CategoryNotifies;

public class SearchCategoryNotifies(
    IDb db,
    ISearchCategories searchCategories)
    : ISearchCategoryNotifies
{
    public IQueryable<CategoryNotify> Respond(ISearchCategoryNotifies.Request request)
    {
        var items = db.Query<CategoryNotify>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.CategoryId, i => i.CategoryId == request.CategoryId);

        if (request.Category is not null)
            items = from i in items join j in searchCategories.Respond(request.Category) on i.CategoryId equals j.Id select i;
        items = items.Filter(request.Title, i => i.Title == request.Title);
        items = items.Filter(request.IsActive, i => i.IsActive == request.IsActive);

        return items.OrderByDescending(i => i.Id);
    }
}
