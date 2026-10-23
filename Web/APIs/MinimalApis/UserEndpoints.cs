using Entities;
using Entities.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Services;
using Services.Models;
using Services.Users;

namespace Web.APIs.MinimalApis;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/user").WithTags("User");

        group.MapPost("/lookup", GetLookup)
            .WithName("GetUserLookup")
            .WithMetadata(new ApiAuthorize())
            .Produces<ApiResponseModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        return group;
    }

    private static IResult GetLookup([FromBody] UserLookupModel? request, ISearchUsers searchUsers, IActorService actorService)
    {
        var actor = actorService.Get();

        if (actor is null || !actor.IsAuthenticated)
            return TypedResults.StatusCode(StatusCodes.Status401Unauthorized);

        if (actor.Type != UserType.Admin)
            return TypedResults.StatusCode(StatusCodes.Status403Forbidden);

        var keyword = request?.Keyword ?? string.Empty;
        var isId = int.TryParse(keyword, out var id);

        var users = searchUsers.Respond(new ISearchUsers.Request
            {
                Id = isId ? id : null,
                Type = request?.Type,
                Keyword = isId ? string.Empty : keyword
            })
            .OrderByDescending(i => i.Id)
            .Take(6)
            .ToList()
            .Select(i => new DropdownModel
            {
                Id = i.Id,
                Title = $"{i.Id} - {i.Name} ({i.Type.GetCaption()})"
            })
            .ToList();

        return TypedResults.Ok(new ApiResponseModel
        {
            Data = users
        });
    }
}

public class UserLookupModel : KeywordModel
{
    public UserType? Type { get; set; }
}