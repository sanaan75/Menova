using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Services;
using Services.Logs;
using Services.TokenDetails;

namespace Web;

public class ApiLogMiddleware(RequestDelegate next)
{
    private const int MaxRequestBodyBytes = 32 * 1024;
    private const string RedactedValue = "[REDACTED]";
    private const string OmittedSensitiveEndpoint = "[omitted: sensitive endpoint]";
    private const string OmittedLargeBody = "[omitted: request body exceeds the log size limit]";
    private const string OmittedUnsupportedBody = "[omitted: unsupported request content type]";
    private const string OmittedUnreadableBody = "[omitted: unreadable request body]";

    public async Task InvokeAsync(HttpContext context, IAddApiLog addApiLog, ITokenDetailCache tokenDetailCache, ITimeService timeService)
    {
        if (!IsApiRequest(context.Request) || HttpMethods.IsOptions(context.Request.Method))
        {
            await next(context);
            return;
        }

        var requestBody = await GetSafeRequestBodyAsync(context.Request);
        var userId = GetUserId(context, tokenDetailCache, timeService);

        try
        {
            await next(context);
        }
        finally
        {
            SaveLog(context, addApiLog, userId, requestBody);
        }
    }

    private static bool IsApiRequest(HttpRequest request)
    {
        var path = request.Path.Value;
        return path is not null && (path.Equals("/api", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<string?> GetSafeRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength is null || request.ContentLength == 0)
            return null;

        if (IsSensitiveEndpoint(request.Path))
            return OmittedSensitiveEndpoint;

        if (request.ContentLength > MaxRequestBodyBytes)
            return OmittedLargeBody;

        if (!IsJson(request.ContentType) && !IsFormUrlEncoded(request.ContentType))
            return OmittedUnsupportedBody;

        try
        {
            request.EnableBuffering();

            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();
            return IsJson(request.ContentType) ? RedactJson(body, request.Path) : RedactForm(body, request.Path);
        }
        catch
        {
            return OmittedUnreadableBody;
        }
        finally
        {
            if (request.Body.CanSeek)
                request.Body.Position = 0;
        }
    }

    private static string RedactJson(string body, PathString path)
    {
        try
        {
            var node = JsonNode.Parse(body);

            if (node is not JsonObject and not JsonArray)
                return OmittedUnreadableBody;

            RedactJsonNode(node, IsSsoCallback(path));
            var result = node.ToJsonString(new JsonSerializerOptions { WriteIndented = false });

            return result.Length <= MaxRequestBodyBytes ? result : OmittedLargeBody;
        }
        catch (JsonException)
        {
            return OmittedUnreadableBody;
        }
    }

    private static void RedactJsonNode(JsonNode? node, bool isSsoCallback)
    {
        switch (node)
        {
            case JsonObject jsonObject:
                foreach (var property in jsonObject.ToArray())
                {
                    if (IsSensitiveField(property.Key, isSsoCallback))
                    {
                        jsonObject[property.Key] = RedactedValue;
                        continue;
                    }

                    RedactJsonNode(property.Value, isSsoCallback);
                }
                break;

            case JsonArray jsonArray:
                for (var index = 0; index < jsonArray.Count; index++)
                {
                    if (jsonArray[index] is JsonValue)
                    {
                        jsonArray[index] = RedactedValue;
                        continue;
                    }

                    RedactJsonNode(jsonArray[index], isSsoCallback);
                }
                break;
        }
    }

    private static string RedactForm(string body, PathString path)
    {
        try
        {
            var isSsoCallback = IsSsoCallback(path);
            var values = QueryHelpers.ParseQuery(body)
                .ToDictionary(
                    item => item.Key,
                    item => IsSensitiveField(item.Key, isSsoCallback)
                        ? new[] { RedactedValue }
                        : item.Value.ToArray(),
                    StringComparer.OrdinalIgnoreCase);

            var result = JsonSerializer.Serialize(values);
            return result.Length <= MaxRequestBodyBytes ? result : OmittedLargeBody;
        }
        catch
        {
            return OmittedUnreadableBody;
        }
    }

    private static bool IsJson(string? contentType)
    {
        return contentType?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) == true ||
               contentType?.Contains("+json", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool IsFormUrlEncoded(string? contentType)
    {
        return contentType?.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool IsSensitiveEndpoint(PathString path)
    {
        var value = path.Value ?? string.Empty;
        return value.Equals("/api/User/Login", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("/api/SSO/Callback", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSsoCallback(PathString path)
    {
        return string.Equals(path.Value, "/api/SSO/Callback", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSensitiveField(string name, bool isSsoCallback)
    {
        var normalized = string.Concat(name.Where(char.IsLetterOrDigit)).ToLowerInvariant();

        if (isSsoCallback && (normalized == "code" || normalized == "state"))
            return true;

        return normalized.Contains("password", StringComparison.Ordinal) ||
               normalized.Contains("token", StringComparison.Ordinal) ||
               normalized.Contains("secret", StringComparison.Ordinal) ||
               normalized.Contains("authorization", StringComparison.Ordinal) ||
               normalized.Contains("cookie", StringComparison.Ordinal) ||
               normalized.Contains("verification", StringComparison.Ordinal) ||
               normalized.Contains("confirmation", StringComparison.Ordinal) ||
               normalized is "otp" or "pin" ||
               normalized.Contains("national", StringComparison.Ordinal) ||
               normalized.Contains("mobile", StringComparison.Ordinal) ||
               normalized.Contains("phone", StringComparison.Ordinal) ||
               normalized.Contains("email", StringComparison.Ordinal) ||
               normalized.Contains("username", StringComparison.Ordinal) ||
               normalized is "firstname" or "lastname" or "fullname";
    }

    private static int? GetUserId(HttpContext context, ITokenDetailCache tokenDetailCache, ITimeService timeService)
    {
        try
        {
            var authorization = context.Request.Headers.Authorization.ToString();

            if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return null;

            var apiKey = authorization["Bearer ".Length..].Trim();
            var token = tokenDetailCache.GetByKey(apiKey);

            return token is { IsActive: true } && token.Expire > timeService.Now
                ? token.UserId
                : null;
        }
        catch
        {
            return null;
        }
    }

    private static void SaveLog(HttpContext context, IAddApiLog addApiLog, int? userId, string? requestBody)
    {
        try
        {
            addApiLog.Respond(new IAddApiLog.Request
            {
                IpAddress = context.Connection.RemoteIpAddress ?? IPAddress.None,
                UserId = userId,
                Path = context.Request.Path.Value ?? string.Empty,
                RequestBody = requestBody
            });
        }
        catch
        {
            // Request logging must not change the API response.
        }
    }
}
