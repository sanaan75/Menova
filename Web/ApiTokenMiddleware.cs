using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Apis;
using Services.TokenDetails;
using Services.Users.Auth;
using Web.APIs;

namespace Web;

public class ApiTokenMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context,
        ITokenValidator tokenValidator,
        ICreateActor createActor,
        IActorService actorService,
        ITokenDetailCache tokenDetailCache)
    {
        var endpoint = context.GetEndpoint();
        var requireAuth = endpoint?.Metadata.GetMetadata<ApiAuthorize>() != null;

        if (!requireAuth)
        {
            await next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].ToString();

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
        var actor = createActor.Respond(token.UserId);
        actorService.Set(actor);

        await next(context);
    }
}