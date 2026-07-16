using Entities;
using Entities.Menus;

namespace Services.Menus.SubCategories;

public class GetSubCategoryInfo(ICheckIsSubCategoryOwner checkIsSubCategoryOwner, ISearchSubCategories searchSubCategories) : IGetSubCategoryInfo
{
    public SubCategory Respond(int id)
    {
        Check.True(checkIsSubCategoryOwner.Respond(id), () => ErrorMessagePersian.NotAllowed(Glossary.GetInfo));

        var subCategory = searchSubCategories.Respond().GetById(id);
        Check.NotNull(subCategory, () => ErrorMessagePersian.NotFound(Glossary.Category));
        return subCategory;
    }
}