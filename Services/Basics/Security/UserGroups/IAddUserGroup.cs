using Entities.Basics.Security;

namespace Services.Basics.Security.UserGroups;

public interface IAddUserGroup
{
    UserGroup Respond(string title);
}