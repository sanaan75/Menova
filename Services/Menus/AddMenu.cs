using Entities;
using Entities.Menus;

namespace Services.Menus;

public class AddMenu(IDb db, ISearchMenus searchMenus) : IAddMenu
{
    public Menu Respond(IAddMenu.Request request)
    {
        Check.True(request.User is not null || request.UserId is > 0, () => ErrorMessagePersian.Unknown(Glossary.User));

        if (request.User is not null && request.UserId is not null)
            Check.True(request.User.Id == request.UserId.Value, () => ErrorMessagePersian.Invalid(Glossary.User));

        var userId = request.User?.Id ?? request.UserId;

        if (userId is > 0)
        {
            var duplicate = searchMenus.Respond(new ISearchMenus.Request
            {
                UserId = userId
            }).Any();

            Check.False(duplicate, () => ErrorMessagePersian.Duplicate(Glossary.Menu));
        }

        var menu = new Menu
        {
            Name = request.Name.Clean(),
            Description = request.Description
        };

        if (request.User is not null)
            menu.User = request.User;
        else
            menu.UserId = request.UserId.Value;

        return db.Set<Menu>().Add(menu).Entity;
    }
}
