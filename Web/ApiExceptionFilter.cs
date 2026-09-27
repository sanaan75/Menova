using Entities;
using Entities.Logs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.TokenDetails;

namespace Web;

public class ApiExceptionFilter(IServiceProvider serviceProvider, ITimeService timeService, ITokenDetailCache tokenDetailCache) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        bool isApiRequest = context.HttpContext.Request.Path.Value?.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) == true;

        if (!isApiRequest)
            return;

        using var services = serviceProvider.CreateScope();

        try
        {
            var db = services.ServiceProvider.GetService<IStatisticDb>();
            var getHost = services.ServiceProvider.GetService<IGetHost>();

            var ipAddress = getHost.Respond();
            int? statusCode = null;
            string appMessage = string.Empty;

            string apiKey = string.Empty;
            int? userId = null;

            var httpContext = context.HttpContext;
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            
            if (string.IsNullOrWhiteSpace(authHeader) == false && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                apiKey = authHeader.Substring("Bearer ".Length).Trim();
                userId = tokenDetailCache.GetByKey(apiKey)?.UserId;
            }

            if (context.Exception is AppException appEx)
            {
                statusCode = 400;
                appMessage = appEx.Message;
            }

            var errorLog = new ErrorLog
            {
                Date = timeService.Now,
                IpAddress = ipAddress,
                UserId = userId,
                Path = context.HttpContext.Request.Path.ToString(),
                StatusCode = statusCode,
                Message = appMessage,
                InnerException = context.Exception.Message + "\t -- " + context.Exception.InnerException?.ToString(),
                StackAndSource = $"{context.Exception.StackTrace}\nSource: {context.Exception.Source}"
            };

            db.Set<ErrorLog>().Add(errorLog);
            db.Save();
        }
        catch (Exception ex)
        {
            // ignored
        }

        if (context.Exception is AppException applicationException)
        {
            context.Result = new ObjectResult(new { Message = applicationException.Message })
            {
                StatusCode = 400
            };
        }
        else
        {
            context.Result = new ObjectResult(new { Message = context.Exception + " " + context.Exception.InnerException })
            {
                StatusCode = 500
            };
        }

        context.ExceptionHandled = true;
    }
}