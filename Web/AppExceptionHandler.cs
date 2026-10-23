using System.Text.Json;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Services.Logs;

namespace Web;

public class AppExceptionHandler(RequestDelegate next, IWebHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context, IAddErrorLog addErrorLog)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (AppException appException)
        {
            var message = string.IsNullOrWhiteSpace(appException.Message) ? "خطایی رخ داده است" : appException.Message;
            Log(context, addErrorLog, StatusCodes.Status400BadRequest, appException, message);

            context.Response.Clear();
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = 400;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, new { message }, options);
        }
        catch (Exception exception)
        {
            if (environment.IsDevelopment())
                throw;

            var isApiRequest = context.Request.Path.Value?.StartsWith("/api/",
                StringComparison.OrdinalIgnoreCase) ?? false;

            if (!isApiRequest)
                throw;

            Log(context, addErrorLog, StatusCodes.Status500InternalServerError, exception, DefaultMessages.AppError());

            context.Response.Clear();
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = 500;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, new { message = DefaultMessages.AppError() }, options);
        }
    }

    private static void Log(HttpContext context, IAddErrorLog addErrorLog, int statusCode, Exception exception, string message)
    {
        addErrorLog.Respond(new IAddErrorLog.Request
        {
            StatusCode = statusCode,
            Path = context.Request.Path.ToString(),
            IpAddress = context.Connection.RemoteIpAddress,
            Message = message,
            Exception = exception
        });
        ErrorLogMiddleware.MarkAsLogged(context);
    }
}
