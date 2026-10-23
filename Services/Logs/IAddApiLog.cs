using System.Net;

namespace Services.Logs;

public interface IAddApiLog
{
    void Respond(Request request);

    class Request
    {
        public IPAddress? IpAddress { get; init; }
        public int? UserId { get; init; }
        public string Path { get; init; } = string.Empty;
        public string? RequestBody { get; init; }
    }
}
