using Entities;
using Entities.Menus;

namespace Services.Menus.Items;

public class AddMenuItem(IDb db, ISearchMenuItems searchMenuItems,IActorService actorService) : IAddMenuItem
{
    public MenuItem Respond(IAddMenuItem.Request request)
    {
        Check.Given(request.Name, () => Glossary.MenuItem);

        var duplicate = searchMenuItems.Respond(new ISearchMenuItems.Request
        {
            SubCategoryId = request.SubCategoryId,
            Name = request.Name.Clean()
        }).Any();
        Check.False(duplicate, () => ErrorMessage.Duplicate(Glossary.MenuItem));

        return db.Set<MenuItem>().Add(new MenuItem
        {
            UserId = actorService.UserId,
            SubCategoryId = request.SubCategoryId,
            Name = request.Name.Clean(),
            Description = request.Description,
            Price = request.Price,
            Order = request.Order,
            IsActive = true
        }).Entity;
    }
}