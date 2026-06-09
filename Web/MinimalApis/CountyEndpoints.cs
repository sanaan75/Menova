using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Services.Basics.AdminLevels;

namespace Web.MinimalApis;

public static class CountyEndpoints
{
    public static RouteGroupBuilder MapCountyEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/").WithTags("");
    
        group.MapGet("", FetchAll2);
    
        return group;
    }
    
    private static async Task<IResult> FetchAll2(ICountyCache countyCache, CancellationToken cancellationToken = default)
    {
        return TypedResults.Ok("hello world");
    }
    
    private static async Task<IResult> FetchAll(ICountyCache countyCache, CancellationToken cancellationToken = default)
    {
        var items = countyCache.GetAll();
    
        return TypedResults.Ok(new { Message = items });
    }
}