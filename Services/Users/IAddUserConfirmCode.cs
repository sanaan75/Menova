using Entities.Users;

namespace Services.Users;

public interface IAddUserConfirmCode
{
    UserConfirmCode Respond(User user);
    UserConfirmCode Respond(int userId);
}