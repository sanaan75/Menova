using Entities;
using Entities.Menus;
using Services.InfraStructures;
using Services.Menus.Items;

namespace Services.Menus.ItemImages;

public class AddMenuItemImage(
    IDb db,
    ISearchMenuItems searchMenuItems,
    IActorService actorService,
    ISaveLocalFile saveLocalFile)
    : IAddMenuItemImage
{
    public MenuItemImage Respond(IAddMenuItemImage.Request request)
    {
        var isItemOwner = searchMenuItems.Respond(new ISearchMenuItems.Request
        {
            Id = request.MenuItemId,
            UserId = actorService.UserId
        }).Any();
        Check.True(isItemOwner, () => ErrorMessagePersian.NotAllowed(Glossary.AddImage));

        var url = saveLocalFile.Respond(new ISaveLocalFile.Request
        {
            File = request.Image,
            Folder = Folders.Categories
        });

        return db.Set<MenuItemImage>().Add(new MenuItemImage
        {
            MenuItemId = request.MenuItemId,
            Url = url
        }).Entity;
    }
}