using System.Collections.Concurrent;

namespace Services.Users.Auth;

public class FailedLoginService : IFailedLoginService
{
    private readonly ConcurrentDictionary<string, int> _failedLogins;
    private readonly ConcurrentDictionary<string, DateTime> _blockedIPs;

    private readonly ITimeService _timeService;
    private readonly IGetFailedLoginBlockTime _getFailedLoginBlockTime;
    private const int maxItem = 6; // todo: rename

    public FailedLoginService(ITimeService timeService, IGetFailedLoginBlockTime getFailedLoginBlockTime)
    {
        _timeService = timeService;
        _getFailedLoginBlockTime = getFailedLoginBlockTime;
        _failedLogins = new ConcurrentDictionary<string, int>();
        _blockedIPs = new ConcurrentDictionary<string, DateTime>();
    }

    public void LogFailedLogin(string ipAddress)
    {
        if (IsIPBlocked(ipAddress) == false)
        {
            _failedLogins.AddOrUpdate(ipAddress, 1, (_, count) => count + 1);

            var count = GetFailedLoginAttempts(ipAddress);
            if (count > maxItem)
                BlockIP(ipAddress);
        }
    }

    public int GetFailedLoginAttempts(string ipAddress)
    {
        _failedLogins.TryGetValue(ipAddress, out var count);
        return count;
    }

    public bool IsIPBlocked(string ipAddress)
    {
        if (_blockedIPs.TryGetValue(ipAddress, out var blockEndTime))
        {
            if (_timeService.Now < blockEndTime)
                return true;

            _blockedIPs.TryRemove(ipAddress, out _);
        }

        return false;
    }

    private void BlockIP(string ipAddress)
    {
        _blockedIPs.AddOrUpdate(ipAddress, _timeService.Now.AddHours(_getFailedLoginBlockTime.Respond()),
            (_, _) => DateTime.Now.AddHours(_getFailedLoginBlockTime.Respond()));
    }
}