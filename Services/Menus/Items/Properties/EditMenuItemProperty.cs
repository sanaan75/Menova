using Entities;
using Entities.Menus;

namespace Services.Menus.Items.Properties;

public class EditMenuItemProperty(IDb db, ISearchMenuItemProperty searchMenuItemProperty) : IEditMenuItemProperty
{
    public void Respond(IEditMenuItemProperty.Request request)
    {
        var property = db.Set<MenuItemProperty>().GetById(request.Id);
        Check.NotNull(property, () => ErrorMessagePersian.NotFound(Glossary.Property));

        var duplicate = searchMenuItemProperty.Respond(new ISearchMenuItemProperty.Request
        {
            IgnoredId = request.Id,
            Title = request.Title.Clean(),
        }).Any();

        Check.False(duplicate, () => ErrorMessage.Duplicate(Glossary.Property));
        property.Title = request.Title.Clean();
        property.Price = request.Price;
    }
}