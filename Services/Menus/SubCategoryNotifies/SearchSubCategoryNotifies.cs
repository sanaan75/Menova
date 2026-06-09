using Entities;
using Entities.Menus;

namespace Services.Menus.SubCategoryNotifies;

public class SearchSubCategoryNotifies(IDb db) : ISearchSubCategoryNotifies
{
    public IQueryable<SubCategoryNotify> Respond(ISearchSubCategoryNotifies.Request request)
    {
        var items = db.Query<SubCategoryNotify>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.SubCategoryId, i => i.SubCategoryId == request.SubCategoryId);
        
        items = items.Filter(request.Title, i => i.Title == request.Title);
        items = items.Filter(request.IsActive, i => i.IsActive == request.IsActive);

        return items.OrderByDescending(i => i.Id);
    }
}