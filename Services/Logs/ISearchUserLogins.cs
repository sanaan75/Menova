using Entities.Logs;

namespace Services.Logs;

public interface ISearchUserLogins
{
    IQueryable<UserLogin> Respond(Request request = null);

    class Request
    {
        public bool ApplyActor { get; set; }
        public int? Id { get; set; }
        public string Username { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public UserLoginMethod? Method { get; set; }
    }
}
