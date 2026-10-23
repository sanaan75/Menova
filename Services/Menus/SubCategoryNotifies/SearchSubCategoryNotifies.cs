using Entities;
using Entities.Menus;

using Services.Menus.SubCategories;

namespace Services.Menus.SubCategoryNotifies;

public class SearchSubCategoryNotifies(IDb db, ISearchSubCategories searchSubCategories) : ISearchSubCategoryNotifies
{
    public IQueryable<SubCategoryNotify> Respond(ISearchSubCategoryNotifies.Request request)
    {
        var items = db.Query<SubCategoryNotify>();
        items = from i in items join j in searchSubCategories.Respond() on i.SubCategoryId equals j.Id select i;

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.SubCategoryId, i => i.SubCategoryId == request.SubCategoryId);

        if (request.SubCategory is not null)
            items = from i in items join j in searchSubCategories.Respond(request.SubCategory) on i.SubCategoryId equals j.Id select i;
        
        items = items.Filter(request.Title, i => i.Title == request.Title);
        items = items.Filter(request.IsActive, i => i.IsActive == request.IsActive);

        return items.OrderByDescending(i => i.Id);
    }
}
