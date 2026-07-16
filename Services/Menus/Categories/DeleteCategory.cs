using Entities;
using Entities.Menus;

namespace Services.Menus.Categories;

public class DeleteCategory(IDb db, ICheckIsCategoryOwner checkIsCategoryOwner) : IDeleteCategory
{
    public void Respond(int id)
    {
        Check.True(checkIsCategoryOwner.Respond(id), () => ErrorMessagePersian.NotAllowed(Glossary.EditCategory));

        var category = db.Set<Category>().GetById(id);
        Check.NotNull(category, () => ErrorMessagePersian.NotFound(Glossary.Category));

        db.Set<Category>().Remove(category);
    }
}