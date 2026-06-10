using Entities;
using Entities.Menus;
using Services.Users;

namespace Services.Menus;

public class SearchMenus(IDb db, ISearchUsers searchUsers) : ISearchMenus
{
    public IQueryable<Menu> Respond(ISearchMenus.Request request)
    {
        var items = db.Query<Menu>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);


        items = items.Filter(request.UserId, i => i.UserId == request.UserId);

        if (request.User is not null)
            items = from i in items join j in searchUsers.Respond(request.User) on i.UserId equals j.Id select i;
        
        items = items.Filter(request.Name, i => i.Name == request.Name);

        return items.OrderByDescending(i => i.Id);
    }
}