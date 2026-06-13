using Entities;
using Entities.Menus;

namespace Services.Menus.SubCategories;

public class EditSubCategory(IDb db, ISearchSubCategories searchSubCategories, ICheckIsSubCategoryOwner checkIsSubCategoryOwner) : IEditSubCategory
{
    public void Respond(IEditSubCategory.Request request)
    {
        Check.Positive(request.Order, () => ErrorMessagePersian.NotAllowed(Glossary.Order));
        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.Name));

        Check.True(checkIsSubCategoryOwner.Respond(request.Id), () => ErrorMessagePersian.NotAllowed(Glossary.EditSubCategory));

        var subCategory = db.Set<SubCategory>().GetById(request.Id);
        Check.NotNull(subCategory, () => ErrorMessagePersian.NotFound(Glossary.SubCategory));

        var duplicate = searchSubCategories.Respond(new ISearchSubCategories.Request
        {
            IgnoredId = request.Id,
            CategoryId = subCategory.CategoryId,
            Name = request.Name.Clean()
        }).Any();

        Check.False(duplicate, () => ErrorMessage.Duplicate(Glossary.SubCategory));

        subCategory.Name = request.Name.Clean();
        subCategory.Description = request.Description;
        subCategory.Order = request.Order;
        subCategory.ImageUrl = request.ImageUrl;
    }
}