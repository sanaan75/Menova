using Entities;
using Entities.Users;

namespace Services.Users.Apis;

public class SearchUserApiToken(
    IDb db,
    IActorService actorService,
    ITimeService timeService)
    : ISearchUserApiToken
{
    public IQueryable<UserApiToken> Respond(ISearchUserApiToken.Request request = null)
    {
        var items = db.Query<UserApiToken>();

        if (request.IsNullOrDefault())
            return items;

        var actor = actorService.Get();
        if (request.ApplyActor)
        {
            if (actor.Type == UserType.SuperAdmin)
                return items;

            items = items.Where(i => i.UserId == actor.UserId);
        }

        if (request.OnlyNotExpired)
        {
            items = items.Where(i => timeService.Now <= i.ExpireDate);
        }

        items = items.Filter(request.UserId, i => i.UserId == actor.UserId);
        items = items.FilterByKeyword(request.Keyword, t => i => i.Note.Contains(t));
        items = items.Filter(request.Token, i => i.Token == request.Token.Value);

        if (request.CreateDate != null)
        {
            var range = request.CreateDate.Value;
            items = items.Filter(range.Start, i => range.Start.Value <= i.CreateDate);
            items = items.Filter(range.End, i => i.CreateDate <= range.End.Value);
        }

        return items.OrderBy(i => i.Id);
    }
}