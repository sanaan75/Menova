using Entities;
using Entities.Users;
using Services;
using Services.Models;
using Services.Users;
using Services.Users.Auth;

namespace Web;

public class CreateActor(
    IDb db,
    IValidatePassword validatePassword,
    ISearchUsers searchUsers,
    IFailedLoginService failedLoginService,
    IUserPermissionService userPermissionService,
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
        
        return new Actor
        {
            IsAuthenticated = true,
            UserId = userId,
            Type = user.Type,
            BusinessType = user.BusinessType,
            Username = user.Username,
            Title = user.Title,
            Name = user.Name,
            Permissions = userPermissionService.GetPermissions(userId).OrderBy(i => (int)i).Select(i => new PermissionModel
                {
                    Value = (int)i,
                    Code = i.ToString(),
                    Title = i.GetCaption()
                })
                .ToList()
        };
    }
}