using Entities;
using Entities.Menus;

namespace Services.Menus.Items.Properties;

public class DeleteMenuItemProperty(IDb db, ICheckIsOwnerProperty checkIsOwnerProperty) : IDeleteMenuItemProperty
{
    public void Respond(int id)
    {
        Check.True(checkIsOwnerProperty.Respond(id), () => ErrorMessagePersian.NotAllowed(Glossary.Delete));

        var property = db.Set<MenuItemProperty>().GetById(id);
        Check.NotNull(property, () => ErrorMessagePersian.NotFound(Glossary.Property));

        db.Set<MenuItemProperty>().Remove(property);
    }
}
