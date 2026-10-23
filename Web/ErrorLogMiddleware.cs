using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Services.Logs;
using Services.TokenDetails;

namespace Web;

public class ErrorLogMiddleware(RequestDelegate next)
{
    public const string LoggedItemKey = "ErrorLogWritten";

    public async Task InvokeAsync(HttpContext context, IAddErrorLog addErrorLog, ITokenDetailCache tokenDetailCache)
    {
        try
        {
            await next(context);

            if (context.Response.StatusCode is 400 or 500)
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                Log(context, addErrorLog, tokenDetailCache, context.Response.StatusCode, exception);
            }
        }
        catch (Exception exception)
        {
            Log(context, addErrorLog, tokenDetailCache, StatusCodes.Status500InternalServerError, exception);
            throw;
        }
    }

    public static void MarkAsLogged(HttpContext context) => context.Items[LoggedItemKey] = true;

    private static void Log(HttpContext context, IAddErrorLog addErrorLog, ITokenDetailCache tokenDetailCache, int statusCode, Exception? exception)
    {
        if (context.Items.ContainsKey(LoggedItemKey))
            return;

        MarkAsLogged(context);

        addErrorLog.Respond(new IAddErrorLog.Request
        {
            StatusCode = statusCode,
            Path = context.Request.Path.ToString(),
            IpAddress = context.Connection.RemoteIpAddress,
            UserId = GetUserId(context, tokenDetailCache),
            Message = exception is null ? $"HTTP {statusCode}" : null,
            Exception = exception
        });
    }

    private static int? GetUserId(HttpContext context, ITokenDetailCache tokenDetailCache)
    {
        try
        {
            var authorization = context.Request.Headers.Authorization.ToString();

            if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return null;

            var apiKey = authorization["Bearer ".Length..].Trim();
            return tokenDetailCache.GetByKey(apiKey)?.UserId;
        }
        catch
        {
            return null;
        }
    }
}