using Entities;
using Entities.Menus;

namespace Services.Menus;

public class AddMenu(IDb db, ISearchMenus searchMenus) : IAddMenu
{
    public Menu Respond(IAddMenu.Request request)
    {
        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.Menu));

        var duplicate = searchMenus.Respond(new ISearchMenus.Request
        {
            UserId = request.UserId,
        }).Any();
        Check.False(duplicate, () => ErrorMessagePersian.Duplicate(Glossary.Menu));

        return db.Set<Menu>().Add(new Menu
        {
            UserId = request.UserId,
            Name = request.Name.Clean(),
            Description = request.Description
        }).Entity;
    }
}