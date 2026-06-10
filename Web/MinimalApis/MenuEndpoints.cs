using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Services.Menus.Items;
using Services.Users;

namespace Web.MinimalApis;

public static class MenuEndpoints
{
    public static RouteGroupBuilder MapMenuEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/cafe").WithTags("Cafe");

        group.MapGet("menu/{username}", FetchAll);

        return group;
    }

    private static async Task<IResult> FetchAll(string username, ISearchUsers searchUsers, ISearchMenuItems searchMenuItems, CancellationToken cancellationToken = default)
    {
        var user = searchUsers.Respond(new ISearchUsers.Request
        {
            Username = username,
        }).Select(i => new { i.Id }).SingleOrDefault();

        if (user == null)
            return Results.NotFound();

        var items = searchMenuItems.Respond(new ISearchMenuItems.Request
        {
            UserId = user.Id
        });

        return TypedResults.Ok(new { Data = items });
    }
}