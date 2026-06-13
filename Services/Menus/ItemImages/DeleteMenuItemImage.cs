using Entities;
using Entities.Menus;

namespace Services.Menus.ItemImages;

public class DeleteMenuItemImage(IDb db, ICheckIsOwnerMenuItemImage checkIsOwnerMenuItemImage) : IDeleteMenuItemImage
{
    public void Respond(int id)
    {
        Check.True(checkIsOwnerMenuItemImage.Respond(id), () => ErrorMessagePersian.NotAllowed(Glossary.Delete));

        var image = db.Set<MenuItemImage>().GetById(id);
        db.Set<MenuItemImage>().Remove(image);
    }
}