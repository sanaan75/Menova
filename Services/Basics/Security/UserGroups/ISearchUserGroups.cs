using Entities.Basics.Security;

namespace Services.Basics.Security.UserGroups;

public interface ISearchUserGroups
{
    IQueryable<UserGroup> Respond(Request request = null);

    class Request
    {
        public string Title { get; set; }
    }
}