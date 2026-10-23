using Entities;
using Entities.Menus;

using Services.Menus.SubCategories;

namespace Services.Menus.Items;

public class AddMenuItem(
    IDb db,
    ISearchMenuItems searchMenuItems,
    IActorService actorService,
    ICheckIsSubCategoryOwner checkIsSubCategoryOwner)
    : IAddMenuItem
{
    public MenuItem Respond(IAddMenuItem.Request request)
    {
        Check.Positive(request.Order, () => ErrorMessagePersian.NotAllowed(Glossary.Order));
        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.MenuItem));

        Check.True(checkIsSubCategoryOwner.Respond(request.SubCategoryId),
            () => ErrorMessagePersian.NotAllowed($"{Glossary.Add} {Glossary.MenuItem}"));

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
            Order = request.Order,
            IsActive = true
        }).Entity;
    }
}
