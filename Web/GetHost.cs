using System.Net;
using Microsoft.AspNetCore.Http;

namespace Web;

public class GetHost : IGetHost
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetHost(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IPAddress? Respond()
    {
        return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
    }
}