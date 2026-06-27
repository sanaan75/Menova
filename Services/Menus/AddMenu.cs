using Entities;
using Entities.Menus;

namespace Services.Menus;

public class AddMenu(IDb db, ISearchMenus searchMenus) : IAddMenu
{
    public Menu Respond(IAddMenu.Request request)
    {
        // var duplicate = searchMenus.Respond(new ISearchMenus.Request
        // {
        //     UserId = request.UserId,
        // }).Any();
        // Check.False(duplicate, () => ErrorMessagePersian.Duplicate(Glossary.Menu));

        var menu = new Menu
        {
            User = request.User,
            Name = request.Name.Clean(),
            Description = request.Description
        };

        if (request.UserId is null)
            menu.User = request.User;
        else
            menu.UserId = request.UserId.Value;

        return db.Set<Menu>().Add(menu).Entity;
    }
}