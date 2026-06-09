using Entities;
using Entities.Users;

namespace Services.Users.Apis;

public class ExpireUserApiToken(IDb db, ITimeService timeService) : IExpireUserApiToken
{
    public void Respond(int id)
    {
        var userApiToken = db.Set<UserApiToken>().GetById(id);

        Check.Ascends(timeService.Now, userApiToken.ExpireDate, () => ErrorMessage.Invalid(Glossary.Token));

        userApiToken.ExpireDate = timeService.Now;
    }
}