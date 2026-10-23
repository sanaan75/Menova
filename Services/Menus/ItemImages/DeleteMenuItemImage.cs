using Entities;
using Entities.Menus;
using Services.InfraStructures;

namespace Services.Menus.ItemImages;

public class DeleteMenuItemImage(
    IDb db,
    ICheckIsOwnerMenuItemImage checkIsOwnerMenuItemImage,
    IRemoveLocalFile removeLocalFile)
    : IDeleteMenuItemImage
{
    public void Respond(int id)
    {
        Check.True(checkIsOwnerMenuItemImage.Respond(id), () => ErrorMessagePersian.NotAllowed(Glossary.Delete));

        var image = db.Set<MenuItemImage>().GetById(id);
        Check.NotNull(image, () => ErrorMessagePersian.NotFound(Glossary.MenuItemImage));

        db.Set<MenuItemImage>().Remove(image);

        if (image.Url is not null && image.Url.StartsWith("/items/", StringComparison.Ordinal))
        {
            db.AddPostSaveAction(() => removeLocalFile.Respond(new IRemoveLocalFile.Request
            {
                Folder = Folders.Items,
                FileUrl = image.Url
            }));
        }
    }
}
