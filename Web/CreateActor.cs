using Entities;
using Entities.Basics.Security;
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
        Check.Given(username, () => CommonMessages.InvalidUsernameOrPassword);

        var user = searchUsers.Respond(new ISearchUsers.Request { Username = username }).SingleOrDefault();

        if (authenticated == false)
        {
            Check.Given(plainPassword, () => CommonMessages.InvalidUsernameOrPassword);
            Check.NotNull(user, () => CommonMessages.InvalidUsernameOrPassword);

            validatePassword.Respond(new IValidatePassword.Request
            {
                Username = user.Username,
                Password = user.Password,
            });
        }

        if (user.Enabled == false)
        {
            failedLoginService.LogFailedLogin(remoteHostService.GetAddress());

            throw new AppException(CommonMessages.InActiveUser);
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
            i.Username
        });

        return new Actor
        {
            IsAuthenticated = true,
            UserId = userId,
            Type = user.Type,
            BusinessType = user.BusinessType,
            Username = user.Username,
            Title =  user.Title,
            Name =  user.Name,
            Permissions = new List<Permission>(),
        };
    }
}