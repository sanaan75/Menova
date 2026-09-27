using Entities.Logs;

namespace Services.Users;

public interface IAddUserLogin
{
    void Respond(Request request);

    class Request
    {
        public string Username { get; set; }
        public UserLoginMethod Method { get; set; }
    }
}