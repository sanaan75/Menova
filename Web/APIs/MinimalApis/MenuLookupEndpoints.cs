using Entities.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Services;
using Services.Menus;
using Services.Menus.Categories;
using Services.Menus.SubCategories;
using Services.Models;

namespace Web.APIs.MinimalApis;

public static class MenuLookupEndpoints
{
    public static IEndpointRouteBuilder MapMenuLookupEndpoints(this IEndpointRouteBuilder routes)
    {
        var categories = routes.MapGroup("/api/category").WithTags("Category");
        categories.MapPost("/lookup", GetCategoryLookup)
            .WithName("GetCategoryLookup")
            .WithMetadata(new ApiAuthorize())
            .Produces<ApiResponseModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        var subCategories = routes.MapGroup("/api/subcategory").WithTags("SubCategory");
        subCategories.MapPost("/lookup", GetSubCategoryLookup)
            .WithName("GetSubCategoryLookup")
            .WithMetadata(new ApiAuthorize())
            .Produces<ApiResponseModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        return routes;
    }

    private static IResult GetCategoryLookup([FromBody] CategoryLookupModel? request, ISearchCategories searchCategories, IActorService actorService)
    {
        var actor = actorService.Get();

        if (actor is null || !actor.IsAuthenticated)
            return TypedResults.StatusCode(StatusCodes.Status401Unauthorized);

        var keyword = request?.Keyword?.Trim();
        var categories = searchCategories.Respond(new ISearchCategories.Request
        {
            MenuId = request?.MenuId,
            Menu = new ISearchMenus.Request
            {
                UserId = actor.UserId
            }
        });

        if (!string.IsNullOrWhiteSpace(keyword))
            categories = categories.Where(i => i.Name.Contains(keyword));

        var items = categories
            .OrderBy(i => i.Order)
            .ThenByDescending(i => i.Id)
            .Take(6)
            .Select(i => new
            {
                i.Id,
                i.Name
            })
            .ToList()
            .Select(i => new DropdownModel
            {
                Id = i.Id,
                Title = $"{i.Id} - {i.Name}"
            })
            .ToList();

        return TypedResults.Ok(new ApiResponseModel
        {
            Data = items
        });
    }

    private static IResult GetSubCategoryLookup(
        [FromBody] SubCategoryLookupModel? request,
        ISearchSubCategories searchSubCategories,
        IActorService actorService)
    {
        var actor = actorService.Get();

        if (actor is null || !actor.IsAuthenticated)
            return TypedResults.StatusCode(StatusCodes.Status401Unauthorized);

        var keyword = request?.Keyword?.Trim();
        var subCategories = searchSubCategories.Respond(new ISearchSubCategories.Request
        {
            Category = request?.CategoryId is not null || request?.MenuId is not null
                ? new ISearchCategories.Request
                {
                    Id = request?.CategoryId,
                    MenuId = request?.MenuId,
                    Menu = new ISearchMenus.Request
                    {
                        UserId = actor.Type == UserType.Admin ? null : actor.UserId
                    }
                }
                : new ISearchCategories.Request
                {
                    Menu = new ISearchMenus.Request
                    {
                        UserId = actor.UserId
                    }
                }
        });

        if (!string.IsNullOrWhiteSpace(keyword))
            subCategories = subCategories.Where(i => i.Name.Contains(keyword));

        var items = subCategories
            .OrderBy(i => i.Order)
            .ThenByDescending(i => i.Id)
            .Take(6)
            .Select(i => new
            {
                i.Id,
                i.Name,
                CategoryName = i.Category.Name
            })
            .ToList()
            .Select(i => new DropdownModel
            {
                Id = i.Id,
                Title = $"{i.Id} - {i.Name} ({i.CategoryName})"
            })
            .ToList();

        return TypedResults.Ok(new ApiResponseModel
        {
            Data = items
        });
    }

    public class CategoryLookupModel : KeywordModel
    {
        public int? MenuId { get; set; }
    }

    public class SubCategoryLookupModel : KeywordModel
    {
        public int? CategoryId { get; set; }
        public int? MenuId { get; set; }
    }
}