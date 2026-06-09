using Entities.Basics.Security;

namespace Services.Basics.Security.UserGroupPermissions;

public interface ISearchUserGroupPermissions
{
    IQueryable<UserGroupPermission> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? UserGroupId { get; set; }
        public Permission? Permission { get; set; }
    }
}