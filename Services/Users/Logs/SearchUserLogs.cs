using Entities;
using Entities.Users;

namespace Services.Users.Logs;

public class SearchUserLogs(IDb db) : ISearchUserLogs
{
    public IQueryable<UserLog> Respond(ISearchUserLogs.Request request = null)
    {
        return Respond(db.Query<UserLog>(), request);
    }

    public IQueryable<UserLog> Respond(IQueryable<UserLog> items, ISearchUserLogs.Request request = null)
    {
        if (Entities.IsNullOrDefaultExt.IsNullOrDefault(request))
            return items;

        items = items.Filter(request!.UserId, i => i.UserId == request.UserId.Value);
        items = items.Filter(request.DoerId, i => i.DoerId == request.DoerId.Value);

        return items;
    }
}
