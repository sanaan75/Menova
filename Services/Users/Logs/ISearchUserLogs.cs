using Entities.Users;

namespace Services.Users.Logs;

public interface ISearchUserLogs
{
    IQueryable<UserLog> Respond(Request request = null);
    IQueryable<UserLog> Respond(IQueryable<UserLog> items, Request request = null);

    class Request
    {
        public int? UserId { get; set; }
        public int? DoerId { get; set; }
    }
}
