using Entities;
using Entities.Users;
using Services;
using Services.Users;
using Services.Users.Auth;

namespace Web;

public class CreateActor(
    IDb db,
    IValidatePassword validatePassword,
    ISearchUsers searchUsers,
    IFailedLoginService failedLoginService,
    IRemoteHostService remoteHostService)
    : ICreateActor
{
    public Actor Respond(string username, string plainPassword, bool authenticated)
    {
        Check.Given(username, () => DefaultMessages.InvalidUsernameOrPassword);

        var user = searchUsers.Respond(new ISearchUsers.Request { Username = username }).SingleOrDefault();
        Check.NotNull(user, () => DefaultMessages.InvalidUsernameOrPassword);

        if (authenticated == false)
        {
            Check.Given(plainPassword, () => DefaultMessages.InvalidUsernameOrPassword);

            validatePassword.Respond(new IValidatePassword.Request
            {
                Username = user.Username,
                Password = user.Password,
            });
        }

        if (user.Enabled == false)
        {
            failedLoginService.LogFailedLogin(remoteHostService.GetAddress());

            throw new AppException(DefaultMessages.InActiveUser);
        }

        return Respond(user.Id);
    }

    public Actor Respond(int userId)
    {
        var user = db.Query<User>().GetById(userId, i => new
        {
            i.Id,
            i.Title,
            i.Name,
            i.CountyId,
            i.Type,
            i.BusinessType,
            i.Username,
            i.Enabled
        });

        Check.NotNull(user, () => DefaultMessages.InActiveUser);
        Check.True(user.Enabled, () => DefaultMessages.InActiveUser);

        var userType = (int)user.Type == 32 ? UserType.Admin : user.Type;
        Check.Defined(userType, () => ErrorMessagePersian.Unknown(Glossary.UserType));
        
        return new Actor
        {
            IsAuthenticated = true,
            UserId = userId,
            Type = userType,
            BusinessType = user.BusinessType,
            Username = user.Username,
            Title = user.Title,
            Name = user.Name
        };
    }
}
