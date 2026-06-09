using System.Net;
using Services;

namespace Web;

public interface IRequestCounters : ISingleInstance
{
    void UpdateRequestCounter(string ipAddress);
    bool IsIPBlocked(string ipAddress, int maxRequestPerMinute);
}