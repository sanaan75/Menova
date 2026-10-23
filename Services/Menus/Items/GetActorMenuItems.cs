using Entities.Menus;

namespace Services.Menus.Items;

public class GetActorMenuItems(IActorService actorService) : IGetActorMenuItems
{
    public IQueryable<MenuItem> Respond(IQueryable<MenuItem> items)
    {
        if (!actorService.IsAuthenticated)
            return items.Where(i => false);

        if (actorService.IsAdmin)
            return items;

        return items.Where(i => i.UserId == actorService.UserId);
    }
}
