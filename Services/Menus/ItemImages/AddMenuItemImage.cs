using Entities;
using Entities.Menus;

namespace Services.Menus.ItemImages;

public class AddMenuItemImage(IDb db) : IAddMenuItemImage
{
    public MenuItemImage Respond(IAddMenuItemImage.Request request)
    {
        Check.Given(request.Url, () => ErrorMessagePersian.Unknown(Glossary.Image));

        return db.Set<MenuItemImage>().Add(new MenuItemImage
        {
            MenuItemId = request.MenuItemId,
            Url = request.Url
        }).Entity;
    }
}