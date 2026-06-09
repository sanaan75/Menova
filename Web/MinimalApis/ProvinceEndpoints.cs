using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Services.Basics;
using Services.Basics.AdminLevels;

namespace Web.MinimalApis;

public static class ProvinceEndpoints
{
    // public static RouteGroupBuilder MapProvinceEndpoints(this IEndpointRouteBuilder routes)
    // {
    //     var group = routes.MapGroup("api/province").WithTags("Province");
    //
    //     group.MapGet("List", FetchAll);
    //
    //     return group;
    // }
    //
    // private static async Task<IResult> FetchAll(IProvinceCache provinceCache, CancellationToken cancellationToken = default)
    // {
    //     var items = provinceCache.GetAll();
    //
    //     return TypedResults.Ok(new { Data = items });
    // }
}