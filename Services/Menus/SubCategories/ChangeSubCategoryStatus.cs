using Entities;
using Entities.Menus;

namespace Services.Menus.SubCategories;

public class ChangeSubCategoryStatus(IDb db, ICheckIsSubCategoryOwner checkIsSubCategoryOwner) : IChangeSubCategoryStatus
{
    public void Respond(int id)
    {
        var subCategory = db.Set<SubCategory>().GetById(id);
        Check.NotNull(subCategory, () => ErrorMessagePersian.NotFound(Glossary.SubCategory));

        Check.True(checkIsSubCategoryOwner.Respond(id), () => ErrorMessagePersian.NotAllowed(Glossary.EditSubCategory));

        subCategory.IsActive = !subCategory.IsActive;
    }
}