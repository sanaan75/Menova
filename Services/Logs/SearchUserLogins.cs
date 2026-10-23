using Entities;
using Entities.Logs;

namespace Services.Logs;

public class SearchUserLogins(IStatisticDb db, IActorService actorService) : ISearchUserLogins
{
    public IQueryable<UserLogin> Respond(ISearchUserLogins.Request request)
    {
        var items = db.Query<UserLogin>();

        if (request.IsNullOrDefault())
            return items;

        items = items.FilterById(request.Id);

        if (request.ApplyActor)
            items = items.Where(i => i.Username == actorService.Get().Username);
        else
            items = items.Filter(request.Username, i => i.Username == request.Username);

        items = items.Filter(request.Method, i => i.Method == request.Method!.Value);

        if (request.DateFrom.HasValue)
            items = items.Where(i => i.Date >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            items = items.Where(i => i.Date <= request.DateTo.Value);

        return items.OrderByDescending(i => i.Id);
    }
}