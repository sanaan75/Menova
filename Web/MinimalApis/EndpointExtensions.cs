using Microsoft.AspNetCore.Builder;

namespace Web.MinimalApis;

public static class EndpointExtensions
{
    public static void MapAllEndpoints(this WebApplication app)
    {
             app.MapMenuEndpoints();
    }
}