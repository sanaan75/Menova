using Entities;
using Entities.Menus;

namespace Services.Menus.CategoryNotifies;

public class SearchCategoryNotifies(IDb db) : ISearchCategoryNotifies
{
    public IQueryable<CategoryNotify> Respond(ISearchCategoryNotifies.Request request)
    {
        var items = db.Query<CategoryNotify>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.CategoryId, i => i.CategoryId == request.CategoryId);
        items = items.Filter(request.Title, i => i.Title == request.Title);
        items = items.Filter(request.IsActive, i => i.IsActive == request.IsActive);

        return items.OrderByDescending(i => i.Id);
    }
}