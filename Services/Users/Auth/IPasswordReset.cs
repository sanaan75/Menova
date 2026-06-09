using Entities.Users;

namespace Services.Users.Auth;

public interface IPasswordReset
{
    void Respond(Request request);

    class Request
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}