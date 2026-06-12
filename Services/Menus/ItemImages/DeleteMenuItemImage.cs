using Entities;
using Entities.Menus;
using Services.Menus.Items;

namespace Services.Menus.ItemImages;

public class DeleteMenuItemImage(IDb db, IActorService actorService,ISearchMenuItems searchMenuItems) : IDeleteMenuItemImage
{
    public void Respond(int id)
    {
        var image = db.Set<MenuItemImage>().GetById(id);
        var userId = actorService.UserId;
        
        var item = searchMenuItems.Respond().GetById(image.MenuItemId);
        // i should check that image is for this user
        db.Set<MenuItemImage>().Remove(image);
    }
}