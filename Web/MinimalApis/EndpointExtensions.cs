using Microsoft.AspNetCore.Builder;
using Web.APIs.MinimalApis;

namespace Web.MinimalApis;

public static class EndpointExtensions
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        app.MapMenuEndpoints();
        app.MapAppEndpoints();
    }
}
