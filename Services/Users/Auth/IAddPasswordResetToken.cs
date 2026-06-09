using Entities.Users;

namespace Services.Users.Auth;

public interface IAddPasswordResetToken
{
    PasswordResetToken Respond(Request request);

    class Request
    {
        public int UserId { get; set; }
    }
}