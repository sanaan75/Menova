using Entities;
using Entities.Menus;
using Services.Menus.Items;

namespace Services.Menus.ItemImages;

public class AddMenuItemImage(IDb db, ISearchMenuItemImages searchMenuItemImages, IActorService actorService) : IAddMenuItemImage
{
    public MenuItemImage Respond(IAddMenuItemImage.Request request)
    {
        Check.Given(request.Url, () => ErrorMessagePersian.Unknown(Glossary.Image));

        var isItemOwner = searchMenuItemImages.Respond(new ISearchMenuItemImages.Request
        {
            MenuItemId = request.MenuItemId,
            MenuItem = new ISearchMenuItems.Request
            {
                UserId = actorService.UserId,
            }
        }).Any();
        Check.True(isItemOwner, () => ErrorMessage.NotAllowed(Glossary.AddImage));

        return db.Set<MenuItemImage>().Add(new MenuItemImage
        {
            MenuItemId = request.MenuItemId,
            Url = request.Url
        }).Entity;
    }
}