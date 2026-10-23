using Entities.Logs;

namespace Services.Logs;

public interface ISearchErrorLogs
{
    IQueryable<ErrorLog> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? StatusCode { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
