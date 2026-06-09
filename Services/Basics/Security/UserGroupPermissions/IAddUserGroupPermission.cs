using Entities.Basics.Security;

namespace Services.Basics.Security.UserGroupPermissions;

public interface IAddUserGroupPermission
{
    UserGroupPermission Respond(int userGroupId, Permission permission);
}