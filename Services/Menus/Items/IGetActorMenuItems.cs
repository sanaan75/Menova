using Entities.Menus;

namespace Services.Menus.Items;

public interface IGetActorMenuItems
{
    IQueryable<MenuItem> Respond(IQueryable<MenuItem> items);
}