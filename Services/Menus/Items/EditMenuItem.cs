using Entities;
using Entities.Menus;

namespace Services.Menus.Items;

public class EditMenuItem(IDb db, ISearchMenuItems searchMenuItems) : IEditMenuItem
{
    public void Respond(IEditMenuItem.Request request)
    {
        Check.Positive(request.Price, () => ErrorMessagePersian.NotAllowed(Glossary.Price));
        Check.Positive(request.Order, () => ErrorMessagePersian.NotAllowed(Glossary.Order));
        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.MenuItem));

        var item = db.Set<MenuItem>().GetById(request.Id);
        Check.NotNull(item, () => ErrorMessagePersian.NotFound(Glossary.MenuItem));

        var duplicate = searchMenuItems.Respond(new ISearchMenuItems.Request
        {
            IgnoredId = request.Id,
            SubCategoryId = item.SubCategoryId,
            Name = request.Name.Clean(),
        }).Any();

        Check.False(duplicate, () => ErrorMessage.Duplicate(Glossary.MenuItem));

        item.Name = request.Name.Clean();
        item.Description = request.Description;
        item.Price = request.Price;
        item.Order = request.Order;
    }
}