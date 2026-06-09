using Entities;
using Entities.Menus;

namespace Services.Menus.SubCategories;

public class ChangeSubCategoryStatus(IDb db) : IChangeSubCategoryStatus
{
    public void Respond(int id)
    {
        var subCategory = db.Set<SubCategory>().GetById(id);
        Check.NotNull(subCategory, () => ErrorMessagePersian.NotFound(Glossary.SubCategory));

        subCategory.IsActive = !subCategory.IsActive;
    }
}