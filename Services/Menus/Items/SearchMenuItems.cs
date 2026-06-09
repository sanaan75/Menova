using Entities;
using Entities.Menus;
using Services.Menus.SubCategories;

namespace Services.Menus.Items;

public class SearchMenuItems(IDb db, ISearchSubCategories searchSubCategories) : ISearchMenuItems
{
    public IQueryable<MenuItem> Respond(ISearchMenuItems.Request request)
    {
        var items = db.Query<MenuItem>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.SubCategoryId, i => i.SubCategoryId == request.SubCategoryId);

        if (request.SubCategory is not null)
            items = from i in items join j in searchSubCategories.Respond(request.SubCategory) on i.SubCategoryId equals j.Id select i;


        items = items.Filter(request.Name, i => i.Name == request.Name);
        items = items.Filter(request.IsActive, i => i.IsActive == request.IsActive);

        return items.OrderByDescending(i => i.Id);
    }
}