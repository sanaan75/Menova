using Entities.Logs;

namespace Services.Logs;

public interface IAddUserLogin
{
    void Respond(Request request);

    class Request
    {
        public string Username { get; set; }
        public UserLoginMethod Method { get; set; }
    }
}
