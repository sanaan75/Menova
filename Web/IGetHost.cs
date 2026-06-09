using System.Net;

namespace Web;

public interface IGetHost
{
    IPAddress? Respond();
}