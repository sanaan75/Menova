using Entities;
using Entities.Menus;
using Services.Menus.SubCategories;

namespace Services.Menus.Categories;

public class DeleteCategory(IDb db, ICheckIsCategoryOwner checkIsCategoryOwner, ISearchSubCategories searchSubCategories) : IDeleteCategory
{
    public void Respond(int id)
    {
        Check.True(checkIsCategoryOwner.Respond(id), () => ErrorMessagePersian.NotAllowed(Glossary.EditCategory));

        var category = db.Set<Category>().GetById(id);
        Check.NotNull(category, () => ErrorMessagePersian.NotFound(Glossary.Category));

        var hasDependencies = searchSubCategories.Respond(new ISearchSubCategories.Request
        {
            CategoryId = category.Id,
        }).Any();
        Check.False(hasDependencies, () => ErrorMessagePersian.Invalid("به علت ثبت زیر دسته ، حذف"));
        
        var notifies = db.Set<CategoryNotify>().Where(i => i.CategoryId == category.Id).ToList();
        db.Set<CategoryNotify>().RemoveRange(notifies);
        db.Set<Category>().Remove(category);
    }
}
