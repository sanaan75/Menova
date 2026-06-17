using System.Text;
using System.Text.Json;
using Entities;
using Entities.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Settings;
using Services.Users.Auth;

namespace Web;

public class LoggingMiddleware(
    IServiceProvider serviceProvider,
    ITimeService timeService,
    ISettingsCache settingsCache,
    IRequestCounters requestCounters,
    IFailedLoginService failedLoginService)
    : IMiddleware
{
    private const int MaxBodySize = 8 * 1024;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using var services = serviceProvider.CreateScope();
        var db = services.ServiceProvider.GetService<IStatisticDb>();
        var getHost = services.ServiceProvider.GetService<IGetHost>();
        var actorService = services.ServiceProvider.GetService<IActorService>();

        try
        {
            var host = getHost.Respond().ToString();
            var settings = settingsCache.Get();

            if (requestCounters.IsIPBlocked(host, settings.AllowedRequestsPerMinute))
            {
                context.Response.StatusCode = 429;
                return;
            }

            requestCounters.UpdateRequestCounter(host);

            if (failedLoginService.IsIPBlocked(host) == true)
            {
                context.Response.StatusCode = 401;
                return;
            }

            var excludedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "/api/file/upload",
                "/api/user/login",
                "/api/user/signup",
                "/api/user/resetPassword",
            };

            var path = context.Request.Path.ToString();

            if (context.Request.Path.StartsWithSegments("/api") && !excludedPaths.Contains(path))
            {
                try
                {
                    var requestBody = await ReadRequestBody(context.Request);

                    var log = new ApiLog
                    {
                        IpAddress = context.Connection.RemoteIpAddress,
                        UserId = actorService?.UserId,
                        Path = context.Request.Path,
                        Date = timeService.Now,
                        RequestBody = SanitizeRequestBody(requestBody)
                    };

                    await db.Set<ApiLog>().AddAsync(log);
                    await db.SaveAsync();
                }
                catch
                {
                    // ignored
                }
            }

            await next(context);
        }
        catch (Exception ex)
        {
            var ipAddress = getHost.Respond();
            int? userId = actorService?.UserId;
            int? statusCode = null;
            string appMessage = string.Empty;

            if (ex is AppException appEx)
            {
                statusCode = 400; // todo : fix a code for app exceptions
                appMessage = appEx.Message;
            }

            var errorLog = new ErrorLog
            {
                Date = timeService.Now,
                IpAddress = ipAddress,
                UserId = userId,
                Path = context.Request.Path.ToString(),
                StatusCode = statusCode,
                Message = appMessage,
                InnerException = ex.Message + "\t -- " + ex.InnerException?.ToString(),
                StackAndSource = $"{ex.StackTrace}\nSource: {ex.Source}",
            };

            try
            {
                await db.Set<ErrorLog>().AddAsync(errorLog);
                await db.SaveAsync();
            }
            catch
            {
                // ignored
            }

            throw;
        }
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        if (request.ContentLength == null || request.ContentLength == 0)
            return string.Empty;

        if (request.ContentLength > MaxBodySize)
            return $"[Body too large: {request.ContentLength} bytes]";

        request.EnableBuffering();

        try
        {
            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            return body;
        }
        catch
        {
            request.Body.Position = 0;
            return "[Error reading body]";
        }
    }

    private string SanitizeRequestBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return body;

        var sensitiveFields = new[]
        {
            "password",
            "token",
            "secret",
            "apikey",
            "authorization",
            "creditcard",
            "cvv",
            "ssn"
        };

        try
        {
            var json = JsonDocument.Parse(body);
            var sanitized = SanitizeJsonElement(json.RootElement, sensitiveFields);
            return JsonSerializer.Serialize(sanitized);
        }
        catch
        {
            return "[Invalid JSON or sanitization failed]";
        }
    }

    private object SanitizeJsonElement(JsonElement element, string[] sensitiveFields)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var obj = new Dictionary<string, object>();
                foreach (var property in element.EnumerateObject())
                {
                    var key = property.Name;
                    var isSensitive = sensitiveFields.Any(f =>
                        key.Contains(f, StringComparison.OrdinalIgnoreCase));

                    obj[key] = isSensitive
                        ? "***REDACTED***"
                        : SanitizeJsonElement(property.Value, sensitiveFields);
                }

                return obj;

            case JsonValueKind.Array:
                return element.EnumerateArray()
                    .Select(e => SanitizeJsonElement(e, sensitiveFields))
                    .ToList();

            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                return element.GetDouble();
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Null:
                return null;
            default:
                return element.ToString();
        }
    }
}