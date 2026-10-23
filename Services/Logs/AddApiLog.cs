using System.Net;
using Entities.Logs;
using Microsoft.Extensions.Logging;

namespace Services.Logs;

public class AddApiLog(IStatisticDb db, ITimeService timeService, ILogger<AddApiLog> logger) : IAddApiLog
{
    private const int MaxPathLength = 1024;
    private const int MaxBodyLength = 32 * 1024;

    public void Respond(IAddApiLog.Request request)
    {
        try
        {
            db.Set<ApiLog>().Add(new ApiLog
            {
                Date = timeService.Now,
                IpAddress = request.IpAddress ?? IPAddress.None,
                UserId = request.UserId,
                Path = Limit(request.Path, MaxPathLength),
                RequestBody = LimitNullable(request.RequestBody, MaxBodyLength)
            });
            db.Save();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to save API request log");
        }
    }

    private static string Limit(string value, int maxLength)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Length <= maxLength ? value : value[..maxLength];
    }

    private static string? LimitNullable(string? value, int maxLength)
    {
        if (value is null)
            return null;

        return value.Length <= maxLength ? value : "[omitted: value exceeds the log size limit]";
    }
}
