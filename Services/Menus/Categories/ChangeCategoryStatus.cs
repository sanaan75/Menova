using Entities;
using Entities.Menus;

namespace Services.Menus.Categories;

public class ChangeCategoryStatus(IDb db, ICheckIsCategoryOwner checkIsCategoryOwner) : IChangeCategoryStatus
{
    public void Respond(int id)
    {
        var category = db.Set<Category>().GetById(id);
        Check.NotNull(category, () => ErrorMessagePersian.NotFound(Glossary.Category));

        Check.True(checkIsCategoryOwner.Respond(id), () => ErrorMessagePersian.NotAllowed(Glossary.EditCategory));

        category.IsActive = !category.IsActive;
    }
}