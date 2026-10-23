using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Services.Logs;
using Services.TokenDetails;

namespace Web;

public class ApiExceptionFilter(ITokenDetailCache tokenDetailCache) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        bool isApiRequest = context.HttpContext.Request.Path.Value?.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) == true;

        if (!isApiRequest)
            return;

        try
        {
            var httpContext = context.HttpContext;
            var addErrorLog = httpContext.RequestServices.GetRequiredService<IAddErrorLog>();
            var statusCode = context.Exception is AppException
                ? StatusCodes.Status400BadRequest
                : StatusCodes.Status500InternalServerError;
            
            var authHeader = httpContext.Request.Headers.Authorization.ToString();
            
            var userId = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? tokenDetailCache.GetByKey(authHeader["Bearer ".Length..].Trim())?.UserId
                : null;

            addErrorLog.Respond(new IAddErrorLog.Request
            {
                StatusCode = statusCode,
                IpAddress = httpContext.Connection.RemoteIpAddress,
                UserId = userId,
                Path = httpContext.Request.Path.ToString(),
                Message = context.Exception is AppException appException ? appException.Message : DefaultMessages.Error500,
                Exception = context.Exception
            });
            ErrorLogMiddleware.MarkAsLogged(httpContext);
        }
        catch
        {
            // Error logging must not change the API response.
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
            context.Result = new ObjectResult(new { Message = DefaultMessages.Error500 })
            {
                StatusCode = 500
            };
        }

        context.ExceptionHandled = true;
    }
}
