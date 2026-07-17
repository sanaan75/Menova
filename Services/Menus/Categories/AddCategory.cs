using Entities;
using Entities.Menus;
using Services.InfraStructures;

namespace Services.Menus.Categories;

public class AddCategory(
    IDb db,
    IActorService actorService,
    ISearchMenus searchMenus,
    ISearchCategories searchCategories,
    ISaveLocalFile saveLocalFile)
    : IAddCategory
{
    public Category Respond(IAddCategory.Request request)
    {
        var menu = searchMenus.Respond(new ISearchMenus.Request
        {
            UserId = actorService.UserId
        }).SingleOrDefault();

        Check.NotNull(menu, () => ErrorMessagePersian.NotFound(Glossary.Menu));
        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.Name));

        var duplicate = searchCategories.Respond(new ISearchCategories.Request
        {
            MenuId = menu.Id,
            Name = request.Name.Clean()
        }).Any();
        Check.False(duplicate, () => ErrorMessagePersian.Duplicate(Glossary.Category));

        var url = saveLocalFile.Respond(new ISaveLocalFile.Request
        {
            File = request.Image,
            Folder = Folders.Categories
        });

        return db.Set<Category>().Add(new Category
        {
            MenuId = menu.Id,
            Name = request.Name.Clean(),
            Description = request.Description,
            Order = request.Order,
            IsActive = true,
            ImageUrl = url
        }).Entity;
    }
}