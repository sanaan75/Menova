using Entities.Users;

namespace Services.Users.Apis;

public class AddUserApiTokenForActor(
    ITimeService timeService,
    IActorService actorService,
    IAddUserApiToken addUserApiToken) 
    : IAddUserApiTokenForActor
{
    public UserApiToken Respond(string note, bool? longLived = false)
    {
        var expireDate = longLived == false ?
            timeService.Now.AddDays(1) :
            timeService.Now.AddDays(90);

        return addUserApiToken.Respond(actorService.UserId, expireDate, note);
    }
}