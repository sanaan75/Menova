using Entities;
using Entities.Logs;
using Services.Sessions;

namespace Services.Users.Auth;

public class Login(
    ISearchUsers searchUsers,
    ICreateActor createActor,
    IActorService actorService,
    ISessionService sessionService,
    IRemoteHostService remoteHostService,
    IFailedLogin failedLogin,
    IAddUserLogin addUserLogin)
    : ILogin
{
    public Actor Respond(string username, string password)
    {
        var ip = remoteHostService.GetAddress();
        Check.False(failedLogin.IsBlocked(username, ip), () => MessageFactory.TemporarilyBlocked("حساب کاربری یا آدرس شما"));

        const string errorMessage = "نام کاربری یا رمز عبور اشتباه است.";

        Check.True(username.Length <= 20 && password.Length <= 20, () => MessageFactory.MaximumLength($"{Glossary.Username} یا {Glossary.Password}", 20));

        var user = searchUsers.Respond(new ISearchUsers.Request
        {
            Username = username
        }).SingleOrDefault();

        if (user == null || HashPassword.Hash(username, password) != user.Password)
        {
            failedLogin.RecordFailure(username, ip);
            throw new AppException(errorMessage);
        }

        var actor = createActor.Respond(user.Username, user.Password, true);
        actorService.Set(actor);
        sessionService.Start(actor, true, user.Username);
        addUserLogin.Respond(new IAddUserLogin.Request
        {
            Username = user.Username,
            Method = UserLoginMethod.Password,
        });

        return actor;
    }
}
