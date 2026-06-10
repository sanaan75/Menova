using System.Text.Json;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Web;

public class AppExceptionHandler(RequestDelegate next, IWebHostEnvironment environment)
{
    private readonly IWebHostEnvironment _environment = environment;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (AppException appException)
        {
            context.Response.Clear();
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = 400;

            var message = string.IsNullOrWhiteSpace(appException.Message) ? "خطایی رخ داده است" : appException.Message;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, new { message }, options);
        }
        catch (Exception)
        {
            if (_environment.IsDevelopment())
                throw;

            var isApiRequest = context.Request.Path.Value?.StartsWith("/api/",
                StringComparison.OrdinalIgnoreCase) ?? false;

            if (!isApiRequest)
                throw;

            context.Response.Clear();
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = 500;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, new { message = CommonMessages.AppError() }, options);
        }
    }
}