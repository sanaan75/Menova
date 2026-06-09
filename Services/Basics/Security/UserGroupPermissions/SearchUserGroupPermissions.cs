using Entities;
using Entities.Basics.Security;

namespace Services.Basics.Security.UserGroupPermissions;

public class SearchUserGroupPermissions(IDb db) : ISearchUserGroupPermissions
{
    public IQueryable<UserGroupPermission> Respond(ISearchUserGroupPermissions.Request request = null)
    {
        var items = db.Query<UserGroupPermission>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);
        
        items = items.Filter(request.UserGroupId, i => i.UserGroupId == request.UserGroupId.Value);
        items = items.Filter(request.Permission, i => i.Permission == request.Permission.Value);

        return items.OrderByDescending(i => i.Id);
    }
}