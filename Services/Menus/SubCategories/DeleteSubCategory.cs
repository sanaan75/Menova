using Entities;
using Entities.Menus;
using Services.Menus.Items;
using Services.Menus.SubCategoryNotifies;

namespace Services.Menus.SubCategories;

public class DeleteSubCategory(
    IDb db,
    ICheckIsSubCategoryOwner checkIsSubCategoryOwner,
    ISearchMenuItems searchMenuItems,
    ISearchSubCategoryNotifies searchSubCategoryNotifies)
    : IDeleteSubCategory
{
    public void Respond(int id)
    {
        Check.True(checkIsSubCategoryOwner.Respond(id), () => ErrorMessagePersian.NotAllowed(Glossary.EditSubCategory));

        var subCategory = db.Set<SubCategory>().GetById(id);
        Check.NotNull(subCategory, () => ErrorMessagePersian.NotFound(Glossary.SubCategory));

        var hasItem = searchMenuItems.Respond(new ISearchMenuItems.Request
        {
            SubCategoryId = subCategory.Id,
        }).Any();

        var hasNotify = searchSubCategoryNotifies.Respond(new ISearchSubCategoryNotifies.Request
        {
            SubCategoryId = subCategory.Id,
        }).Any();
        
        Check.False(hasNotify && hasItem, () => ErrorMessagePersian.Invalid("به علت ثبت آیتم یا نوتفیکیشن ، حذف"));

        db.Set<SubCategory>().Remove(subCategory);
    }
}