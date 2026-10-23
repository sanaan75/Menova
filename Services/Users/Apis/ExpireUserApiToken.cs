using Entities;
using Entities.Users;

namespace Services.Users.Apis;

public class ExpireUserApiToken(IDb db, ITimeService timeService, IActorService actorService) : IExpireUserApiToken
{
    public void Respond(int id)
    {
        var userApiToken = db.Set<UserApiToken>().GetById(id);
        Check.NotNull(userApiToken, () => ErrorMessagePersian.NotFound(Glossary.Token));
        Check.True(actorService.IsAdmin || userApiToken.UserId == actorService.UserId,
            () => ErrorMessagePersian.NotAllowed(Glossary.Remove));

        Check.Ascends(timeService.Now, userApiToken.ExpireDate, () => ErrorMessage.Invalid(Glossary.Token));

        userApiToken.ExpireDate = timeService.Now;
    }
}
