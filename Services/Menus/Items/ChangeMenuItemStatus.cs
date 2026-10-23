using Entities;
using Entities.Menus;

namespace Services.Menus.Items;

public class ChangeMenuItemStatus(IDb db, ISearchMenuItems searchMenuItems) : IChangeMenuItemStatus
{
    public void Respond(int id)
    {
        var item = db.Set<MenuItem>().GetById(id);
        Check.NotNull(item, () => ErrorMessagePersian.NotFound(Glossary.MenuItem));
        Check.True(searchMenuItems.Respond(new ISearchMenuItems.Request
        {
            Id = id
        }).Any(), () => ErrorMessagePersian.NotAllowed(Glossary.Edit));

        item.IsActive = !item.IsActive;
    }
}
