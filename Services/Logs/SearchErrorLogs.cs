using Entities;
using Entities.Logs;

namespace Services.Logs;

public class SearchErrorLogs(IStatisticDb db) : ISearchErrorLogs
{
    public IQueryable<ErrorLog> Respond(ISearchErrorLogs.Request request)
    {
        var items = db.Query<ErrorLog>();

        if (request.IsNullOrDefault() == false)
        {
            items = items.FilterById(request.Id);
            items = items.Filter(request.UserId, i => i.UserId == request.UserId!.Value);
            items = items.Filter(request.StatusCode, i => i.StatusCode == request.StatusCode!.Value);

            if (request.DateFrom.HasValue)
                items = items.Where(i => i.Date >= request.DateFrom.Value);

            if (request.DateTo.HasValue)
                items = items.Where(i => i.Date <= request.DateTo.Value);
        }

        return items.OrderByDescending(i => i.Date).ThenByDescending(i => i.Id);
    }
}
