using Entities;
using Entities.Menus;
using Services.InfraStructures;
using Services.Menus.Categories;

namespace Services.Menus.SubCategories;

public class AddSubCategory(
    IDb db,
    ISearchSubCategories searchSubCategories,
    IActorService actorService,
    ICheckIsCategoryOwner checkIsCategoryOwner,
    ISaveLocalFile saveLocalFile)
    : IAddSubCategory
{
    public SubCategory Respond(IAddSubCategory.Request request)
    {
        Check.Positive(request.Order, () => ErrorMessagePersian.NotAllowed(Glossary.Order));
        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.Name));

        Check.True(checkIsCategoryOwner.Respond(request.CategoryId), () => ErrorMessagePersian.NotAllowed($"{Glossary.Add} {Glossary.SubCategory}"));

        var duplicate = searchSubCategories.Respond(new ISearchSubCategories.Request
        {
            CategoryId = request.CategoryId,
            Name = request.Name.Clean(),
            Category = new ISearchCategories.Request
            {
                Menu = new ISearchMenus.Request
                {
                    UserId = actorService.UserId
                }
            }
        }).Any();

        Check.False(duplicate, () => ErrorMessagePersian.Duplicate(Glossary.SubCategory));

        var url = saveLocalFile.Respond(new ISaveLocalFile.Request
        {
            File = request.Image,
            Folder = Folders.Categories
        });

        return db.Set<SubCategory>().Add(new SubCategory
        {
            CategoryId = request.CategoryId,
            Name = request.Name.Clean(),
            Description = request.Description,
            Order = request.Order,
            ImageUrl = url,
            IsActive = true
        }).Entity;
    }
}