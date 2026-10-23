using Entities.Logs;
using Microsoft.Extensions.Logging;

namespace Services.Logs;

public class AddErrorLog(IStatisticDb db, ITimeService timeService, ILogger<AddErrorLog> logger) : IAddErrorLog
{
    public void Respond(IAddErrorLog.Request request)
    {
        try
        {
            var exception = request.Exception;
            var shouldStoreExceptionDetails = request.StatusCode != 400;

            db.Set<ErrorLog>().Add(new ErrorLog
            {
                Date = timeService.Now,
                IpAddress = request.IpAddress,
                UserId = request.UserId,
                Path = request.Path ?? string.Empty,
                StatusCode = request.StatusCode,
                Message = string.IsNullOrWhiteSpace(request.Message) ? exception?.Message : request.Message,
                InnerException = shouldStoreExceptionDetails ? exception?.InnerException?.ToString() : null,
                StackAndSource = shouldStoreExceptionDetails && exception is not null
                    ? $"{exception.StackTrace}\nSource: {exception.Source}"
                    : null
            });
            db.Save();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to save error log");
        }
    }
}
