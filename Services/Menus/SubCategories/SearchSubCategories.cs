using Entities;
using Entities.Menus;
using Services.Menus.Categories;

namespace Services.Menus.SubCategories;

public class SearchSubCategories(IDb db,ISearchCategories searchCategories) : ISearchSubCategories
{
    public IQueryable<SubCategory> Respond(ISearchSubCategories.Request request)
    {
        var items = db.Query<SubCategory>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.CategoryId, i => i.CategoryId == request.CategoryId);
        
        if (request.Category is not null)
            items = from i in items join j in searchCategories.Respond(request.Category) on i.CategoryId equals j.Id select i;

        
        items = items.Filter(request.Name, i => i.Name == request.Name);
        items = items.Filter(request.IsActive, i => i.IsActive == request.IsActive);

        return items.OrderBy(i => i.Order).ThenByDescending(i => i.Id);
    }
}