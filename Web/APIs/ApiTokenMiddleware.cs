using System.Security.Claims;
using Entities;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Apis;
using Services.TokenDetails;
using Services.Users.Auth;

namespace Web.APIs;

public class ApiTokenMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context,
        ITokenValidator tokenValidator,
        ICreateActor createActor,
        IActorService actorService,
        ITokenDetailCache tokenDetailCache)
    {
        if (context.Request.Method == HttpMethods.Options)
        {
            await next(context);
            return;
        }
        
        var endpoint = context.GetEndpoint();
        var requireAuth = endpoint?.Metadata.GetMetadata<ApiAuthorize>() != null;

        if (!requireAuth)
        {
            await next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Missing or invalid Authorization header.");
            return;
        }

        var apiKey = authHeader.Substring("Bearer ".Length).Trim();
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var isValid = await tokenValidator.ValidateTokenAsync(apiKey, principal);

        if (!isValid)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid or expired API token.");
            return;
        }

        var token = tokenDetailCache.GetByKey(apiKey);
        if (token is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid or expired API token.");
            return;
        }

        Actor actor;
        try
        {
            actor = createActor.Respond(token.UserId);
        }
        catch (AppException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("User account is inactive or unavailable.");
            return;
        }

        actorService.Set(actor);

        await next(context);
    }
}
