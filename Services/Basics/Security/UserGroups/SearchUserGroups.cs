using Entities.Basics.Security;

namespace Services.Basics.Security.UserGroups;

public class SearchUserGroups(IDb db) : ISearchUserGroups
{
    public IQueryable<UserGroup> Respond(ISearchUserGroups.Request request)
    {
        var items = db.Query<UserGroup>();

        if (request.IsNullOrDefault())
            return items;

        if (string.IsNullOrWhiteSpace(request.Title) == false)
            items = items.Where(item => item.Title == request.Title);

        return items.OrderByDescending(i => i.Id);
    }
}