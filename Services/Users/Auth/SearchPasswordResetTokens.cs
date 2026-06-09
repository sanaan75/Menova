using Entities;
using Entities.Users;

namespace Services.Users.Auth;

public class SearchPasswordResetTokens(IDb db) : ISearchPasswordResetTokens
{
    public IQueryable<PasswordResetToken> Respond(ISearchPasswordResetTokens.Request request = null)
    {
        var items = db.Query<PasswordResetToken>();

        if (request.IsNullOrDefault())
            return items;

        return Respond(items, request);
    }

    public IQueryable<PasswordResetToken> Respond(IQueryable<PasswordResetToken> items, ISearchPasswordResetTokens.Request request = null)
    {
        if (request.IsNullOrDefault())
            return items;

        items.FilterById(request.Id);
        items.IgnoreById(request.IgnoredId);

        items.Filter(request.UserId, i => i.UserId == request.UserId);

        if (string.IsNullOrWhiteSpace(request.Token) == false)
            items = items.Where(i => i.Token.Equals(request.Token));

        if (request.Used is not null)
            items.Where(i => i.Used == request.Used);

        return items.OrderByDescending(i => i.Id);
    }
}