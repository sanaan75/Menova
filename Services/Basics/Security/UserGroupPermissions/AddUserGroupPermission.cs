using Entities;
using Entities.Basics.Security;
using Services.Basics.Security.UserGroups;

namespace Services.Basics.Security.UserGroupPermissions;

public class AddUserGroupPermission(
    IDb db,
    ISearchUserGroups searchUserGroups,
    ISearchUserGroupPermissions searchUserGroupPermissions,
    IUserGroupCache userGroupCache)
    : IAddUserGroupPermission
{
    public UserGroupPermission Respond(int userGroupId, Permission permission)
    {
        Check.Defined(permission, () => ErrorMessage.Unknown(Glossary.Permission));

        var userGroup = searchUserGroups.Respond().GetById(userGroupId, i => new { i.Id });
        Check.NotNull(userGroup, () => ErrorMessage.NotFound(Glossary.UserGroup));

        var isDuplicate = searchUserGroupPermissions.Respond(new ISearchUserGroupPermissions.Request
        {
            UserGroupId = userGroupId,
            Permission = permission
        }).Any();

        Check.False(isDuplicate, () => ErrorMessage.Duplicate(Glossary.Permission));

        db.AddPostSaveAction(userGroupCache.Reset);

        return db.Set<UserGroupPermission>().Add(new UserGroupPermission
        {
            UserGroupId = userGroupId,
            Permission = permission
        }).Entity;
    }
}