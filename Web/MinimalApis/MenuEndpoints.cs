using Entities.Menus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Services;
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

    private static async Task<IResult> FetchAll(
        string username,
        IDb db,
        ISearchUsers searchUsers,
        CancellationToken cancellationToken = default)
    {
        var userId = searchUsers.Respond(new ISearchUsers.Request
            {
                Username = username
            })
            .Select(u => u.Id)
            .SingleOrDefault();

        if (userId == 0)
            return Results.NotFound();

        var items = await db.Query<MenuItem>()
            .Where(i => i.UserId == userId && i.SubCategory.IsActive && i.SubCategory.Category.IsActive)
            .Select(i => new
            {
                i.Id,
                i.Name,
                i.Description,
                i.Order,

                Properties = i.Properties.Select(p => new
                {
                    p.Id, p.Title, p.Price,
                }),

                Images = i.Images.Select(img => new
                {
                    img.Id, ImageUrl = img.Url
                }),

                SubCategory = new
                {
                    Id = i.SubCategoryId,
                    i.SubCategory.Name,
                    i.SubCategory.Description,
                    i.SubCategory.Order,

                    Notifies = i.SubCategory.Notifies.Select(n => new
                    {
                        n.Id,
                        n.Title
                    }),

                    Category = new
                    {
                        Id = i.SubCategory.CategoryId,
                        i.SubCategory.Category.Name,
                        i.SubCategory.Category.Description,
                        i.SubCategory.Category.Order,
                        i.SubCategory.Category.ImageUrl,

                        Notifies = i.SubCategory.Category.Notifies.Select(n => new
                        {
                            n.Id,
                            n.Title
                        }),

                        Menu = new
                        {
                            i.SubCategory.Category.Menu.Name,
                            i.SubCategory.Category.Menu.Description
                        }
                    }
                }
            })
            .ToListAsync(cancellationToken: cancellationToken);

        return TypedResults.Ok(new { Data = items });
    }
}