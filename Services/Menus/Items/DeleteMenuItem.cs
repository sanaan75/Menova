using Entities;
using Entities.Menus;
using Services.InfraStructures;

namespace Services.Menus.Items;

public class DeleteMenuItem(IDb db, ISearchMenuItems searchMenuItems, IRemoveLocalFile removeLocalFile) : IDeleteMenuItem
{
    public void Respond(int id)
    {
        var item = db.Set<MenuItem>().GetById(id);
        Check.NotNull(item, () => ErrorMessagePersian.NotFound(Glossary.MenuItem));
        Check.True(searchMenuItems.Respond(new ISearchMenuItems.Request
        {
            Id = id
        }).Any(), () => ErrorMessagePersian.NotAllowed(Glossary.Delete));

        var images = db.Set<MenuItemImage>().Where(i => i.MenuItemId == id).ToList();
        var properties = db.Set<MenuItemProperty>().Where(i => i.MenuItemId == id).ToList();

        db.Set<MenuItemImage>().RemoveRange(images);
        db.Set<MenuItemProperty>().RemoveRange(properties);
        db.Set<MenuItem>().Remove(item);

        var imageUrls = images
            .Select(i => i.Url)
            .Where(i => i is not null && i.StartsWith("/items/", StringComparison.Ordinal))
            .ToList();

        if (imageUrls.Count > 0)
        {
            db.AddPostSaveAction(() =>
            {
                foreach (var imageUrl in imageUrls)
                {
                    removeLocalFile.Respond(new IRemoveLocalFile.Request
                    {
                        Folder = Folders.Items,
                        FileUrl = imageUrl
                    });
                }
            });
        }
    }
}
