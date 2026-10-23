using Microsoft.AspNetCore.Routing;

namespace Web.APIs.MinimalApis;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapAppEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUserEndpoints();

        return app;
    }
}
