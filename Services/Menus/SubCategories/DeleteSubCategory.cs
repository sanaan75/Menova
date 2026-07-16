using Entities;
using Entities.Menus;

namespace Services.Menus.SubCategories;

public class DeleteSubCategory(IDb db , ICheckIsSubCategoryOwner checkIsSubCategoryOwner) : IDeleteSubCategory
{
    public void Respond(int id)
    {
        Check.True(checkIsSubCategoryOwner.Respond(id), () => ErrorMessagePersian.NotAllowed(Glossary.EditSubCategory));

        var subCategory = db.Set<SubCategory>().GetById(id);
        Check.NotNull(subCategory, () => ErrorMessagePersian.NotFound(Glossary.SubCategory));

        db.Set<SubCategory>().Remove(subCategory);
    }
}