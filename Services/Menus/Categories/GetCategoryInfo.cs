using Entities;
using Entities.Menus;

namespace Services.Menus.Categories;

public class GetCategoryInfo(ICheckIsCategoryOwner checkIsCategoryOwner, ISearchCategories searchCategories) : IGetCategoryInfo
{
    public Category Respond(int id)
    {
        Check.True(checkIsCategoryOwner.Respond(id), () => ErrorMessagePersian.NotAllowed(Glossary.GetInfo));

        var category = searchCategories.Respond().GetById(id);
        Check.NotNull(category, () => ErrorMessagePersian.NotFound(Glossary.Category));
        return category;
    }
}