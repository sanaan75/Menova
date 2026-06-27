using Entities;
using Entities.Users;

namespace Services.Users;

public class SearchUsers(IDb db) : ISearchUsers
{
    public IQueryable<User> Respond(ISearchUsers.Request request)
    {
        var items = db.Query<User>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        items = items.IgnoreById(request.IgnoredId);

        items = items.Filter(request.Type, i => i.Type == request.Type!.Value);
        items = items.Filter(request.BusinessType, i => i.BusinessType == request.BusinessType!.Value);
        
        items = items.Filter(request.CountyId, i => i.CountyId == request.CountyId!.Value);
        items = items.Filter(request.Username, i => i.Username == request.Username);
        items = items.Filter(request.Slug, i => i.Slug == request.Slug);

        if (request.Enabled is not null)
            items = items.Where(i => i.Enabled == request.Enabled.Value);

        return items.OrderByDescending(i => i.Id);
    }
}