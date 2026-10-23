using Entities;
using Entities.Menus;
using Services.Menus.SubCategories;

namespace Services.Menus.Items;

public class AddMenuItem(IDb db, ISearchMenuItems searchMenuItems, ISearchSubCategories searchSubCategories, ICheckIsSubCategoryOwner checkIsSubCategoryOwner) : IAddMenuItem
{
    public MenuItem Respond(IAddMenuItem.Request request)
    {
        Check.Positive(request.Order, () => ErrorMessagePersian.NotAllowed(Glossary.Order));
        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.MenuItem));

        Check.True(checkIsSubCategoryOwner.Respond(request.SubCategoryId), () => ErrorMessagePersian.NotAllowed($"{Glossary.Add} {Glossary.MenuItem}"));

        var duplicate = searchMenuItems.Respond(new ISearchMenuItems.Request
        {
            SubCategoryId = request.SubCategoryId,
            Name = request.Name.Clean()
        }).Any();
        Check.False(duplicate, () => ErrorMessage.Duplicate(Glossary.MenuItem));

        var userId = searchSubCategories.Respond(new ISearchSubCategories.Request
            {
                Id = request.SubCategoryId
            })
            .Select(i => i.Category.Menu.UserId)
            .SingleOrDefault();

        return db.Set<MenuItem>().Add(new MenuItem
        {
            UserId = userId,
            SubCategoryId = request.SubCategoryId,
            Name = request.Name.Clean(),
            Description = request.Description,
            Order = request.Order,
            IsActive = true
        }).Entity;
    }
}