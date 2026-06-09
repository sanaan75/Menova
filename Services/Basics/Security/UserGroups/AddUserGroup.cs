using Entities;
using Entities.Basics.Security;

namespace Services.Basics.Security.UserGroups;

public class AddUserGroup(IDb db, ISearchUserGroups searchUserGroups, IUserGroupCache userGroupCache) : IAddUserGroup
{
    public UserGroup Respond(string title)
    {
        var cleanedTitle = title.Clean();
        Check.Given(cleanedTitle, () => ErrorMessage.Unknown(Glossary.Title));

        var isDuplicate = searchUserGroups.Respond(new ISearchUserGroups.Request
        {
            Title = cleanedTitle
        }).Any();

        Check.False(isDuplicate, () => ErrorMessage.Duplicate(Glossary.UserGroup));

        db.AddPostSaveAction(userGroupCache.Reset);

        return db.Set<UserGroup>().Add(new UserGroup { Title = cleanedTitle }).Entity;
    }
}