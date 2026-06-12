using Entities;
using Entities.Menus;

namespace Services.Menus.Items.Properties;

public class AddMenuItemProperty(IDb db, ISearchMenuItemProperty searchMenuItemProperty) : IAddMenuItemProperty
{
    public MenuItemProperty Respond(IAddMenuItemProperty.Request request)
    {
        var duplicate = searchMenuItemProperty.Respond(new ISearchMenuItemProperty.Request
        {
            MenuItemId = request.MenuItemId,
            Title = request.Title.Clean(),
        }).Any();
        Check.False(duplicate, () => ErrorMessage.Duplicate(Glossary.Property));

        return db.Set<MenuItemProperty>().Add(new MenuItemProperty
        {
            MenuItemId = request.MenuItemId,
            Title = request.Title.Clean(),
            Price = request.Price,
        }).Entity;
    }
}