namespace Services.Logs;

public interface ICleanupActionLogs
{
    Task<Response> Respond(CancellationToken cancellationToken = default);

    class Response
    {
        public int DeletedByAge { get; set; }
        public bool IsSkipped { get; set; }
        public int DeletedCount => DeletedByAge;
    }
}
