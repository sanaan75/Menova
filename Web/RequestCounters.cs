using System.Collections.Concurrent;
using System.Net;
using Services;

namespace Web;

public class RequestCounters(ITimeService timeService) : IRequestCounters
{
    private readonly ConcurrentDictionary<string, (int Count, DateTime ResetTime)> _requestCounters = new();

    public bool IsIPBlocked(string ipAddress, int maxRequestPerMinute)
    {
        if (_requestCounters.TryGetValue(ipAddress, out var counter))
        {
            if (timeService.Now < counter.ResetTime && counter.Count >= maxRequestPerMinute)
                return true;
        }

        return false;
    }

    public void UpdateRequestCounter(string ipAddress)
    {
        var currentTime = timeService.Now;

        _requestCounters.AddOrUpdate(ipAddress, key => (1, currentTime.AddMinutes(1)), (key, value) =>
            {
                if (currentTime < value.ResetTime)
                    return (value.Count + 1, value.ResetTime);

                return (1, currentTime.AddMinutes(1));
            }
        );
    }
}