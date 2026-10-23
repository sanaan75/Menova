using System.Net;

namespace Services.Logs;

public interface IAddErrorLog
{
    void Respond(Request request);

    class Request
    {
        public int StatusCode { get; init; }
        public string Path { get; init; } = string.Empty;
        public IPAddress? IpAddress { get; init; }
        public int? UserId { get; init; }
        public string? Message { get; init; }
        public Exception? Exception { get; init; }
    }
}
