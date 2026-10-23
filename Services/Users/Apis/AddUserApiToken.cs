using Entities;
using Entities.Users;

namespace Services.Users.Apis;

public class AddUserApiToken(
    IDb db,
    IActorService actorService,
    ITimeService timeService,
    IRemoteHostService remoteHostService,
    IExpireUserApiToken expireUserApiToken,
    ISearchUserApiToken searchUserApiToken)
    : IAddUserApiToken
{
    public UserApiToken Respond(int userId, DateTime expireDate, string note)
    {
        Check.Positive(userId, () => ErrorMessagePersian.Unknown(Glossary.User));
        Check.True(actorService.IsAdmin || actorService.UserId == userId, () => ErrorMessagePersian.NotAllowed(Glossary.Add));
        Check.Ascends(timeService.Now, expireDate, () => ErrorMessage.NotAllowed(Glossary.ExpireDate));
        Check.Given(note, () => ErrorMessage.Unknown(Glossary.Note));

        var otherToken = searchUserApiToken.Respond(new ISearchUserApiToken.Request
            {
                UserId = userId,
                OnlyNotExpired = true
            })
            .Select(i => new
            {
                i.Id
            })
            .SingleOrDefault();

        if (otherToken != null)
            expireUserApiToken.Respond(otherToken.Id);

        return db.Set<UserApiToken>().Add(new UserApiToken
        {
            UserId = userId,
            CreateDate = timeService.Now,
            ExpireDate = expireDate,
            Token = Guid.NewGuid(),
            Host = remoteHostService.GetAddress(),
            Note = note
        }).Entity;
    }
}