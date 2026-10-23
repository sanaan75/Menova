using Services.Menus.Items;

namespace Services.Menus.ItemImages;

public class CheckIsOwnerMenuItemImage(ISearchMenuItemImages searchMenuItemImages, IActorService actorService) : ICheckIsOwnerMenuItemImage
{
    public bool Respond(int id)
    {
        var request = new ISearchMenuItemImages.Request
        {
            Id = id,
        };

        if (!actorService.IsAdmin)
        {
            request.MenuItem = new ISearchMenuItems.Request
            {
                UserId = actorService.UserId
            };
        }

        return searchMenuItemImages.Respond(request).Any();
    }
}
