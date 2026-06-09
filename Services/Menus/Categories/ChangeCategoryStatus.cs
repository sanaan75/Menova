using Entities;
using Entities.Menus;

namespace Services.Menus.Categories;

public class ChangeCategoryStatus(IDb db) : IChangeCategoryStatus
{
    public void Respond(int id)
    {
        var category = db.Set<Category>().GetById(id);
        Check.NotNull(category, () => ErrorMessagePersian.NotFound(Glossary.Category));

        category.IsActive = !category.IsActive;
    }
}