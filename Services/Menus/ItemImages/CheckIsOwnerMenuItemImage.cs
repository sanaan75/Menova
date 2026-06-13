using Services.Menus.Items;

namespace Services.Menus.ItemImages;

public class CheckIsOwnerMenuItemImage(ISearchMenuItemImages searchMenuItemImages, IActorService actorService) : ICheckIsOwnerMenuItemImage
{
    public bool Respond(int id)
    {
        return searchMenuItemImages.Respond(new ISearchMenuItemImages.Request
        {
            Id = id,
            MenuItem = new ISearchMenuItems.Request
            {
                UserId = actorService.UserId
            }
        }).Any();
    }
}