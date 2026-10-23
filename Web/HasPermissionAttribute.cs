using Entities.Basics.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Services;

public sealed class HasPermissionAttribute(params Permission[] permissions) : Attribute, IAuthorizationFilter
{
    public Permission[] Permissions { get; } = permissions;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var actorService = context.HttpContext.RequestServices.GetRequiredService<IActorService>();

        var actor = actorService.Get();

        if (actor is null || !actor.IsAuthenticated)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }

        foreach (var permission in Permissions)
        {
            if (!actor.HasPermission(permission))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }
        }
    }
}
