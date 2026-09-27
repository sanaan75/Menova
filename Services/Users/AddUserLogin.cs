using System.Net;
using Entities.Logs;
using Microsoft.Extensions.Logging;

namespace Services.Users;

public class AddUserLogin(
    IStatisticDb db,
    ITimeService timeService,
    IRemoteHostService remoteHostService,
    ILogger<AddUserLogin> logger)
    : IAddUserLogin
{
    public void Respond(IAddUserLogin.Request request)
    {
        try
        {
            db.Set<UserLogin>().Add(new UserLogin
            {
                Username = request.Username,
                Date = timeService.Now,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent(),
                Method = request.Method,
            });

            db.Save();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to save user login history");
        }
    }

    private string? GetIpAddress()
    {
        var forwardedAddress = remoteHostService.GetAddress()?.Split(',').FirstOrDefault()?.Trim();
        if (IPAddress.TryParse(forwardedAddress, out var ipAddress))
            return ipAddress.ToString();

        return remoteHostService.GetIpAddress()?.ToString();
    }

    private string? GetUserAgent()
    {
        const int maxLength = 1000;
        var userAgent = remoteHostService.GetAgent();

        return userAgent?.Length > maxLength ? userAgent[..maxLength] : userAgent;
    }
}