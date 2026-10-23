using Entities;
using Entities.Basics.Security;

namespace Services.Users.Auth;

public class UserPermissionService(IDb db) : IUserPermissionService
{
    public IReadOnlyCollection<Permission> GetPermissions(int userId)
    {
        var roleIds = GetRoleIds(userId);

        if (roleIds.Count == 0)
            return [];

        return db.Query<UserGroupPermission>()
            .Where(i => roleIds.Contains(i.UserGroupId))
            .Select(i => i.Permission)
            .Distinct()
            .OrderBy(i => i)
            .ToList();    }

    public bool HasPermission(int userId, Permission permission)
    {
        return GetPermissions(userId).Any(i => i.Includes(permission));
    }
    

    private List<int> GetRoleIds(int userId)
    {
        return db.Query<UserInGroup>().Where(i => i.UserId==userId).Select(i => i.UserGroupId).Distinct().ToList();
    }
}
