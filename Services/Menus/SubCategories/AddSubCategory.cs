using Entities;
using Entities.Menus;

namespace Services.Menus.SubCategories;

public class AddSubCategory(IDb db, ISearchSubCategories searchSubCategories) : IAddSubCategory
{
    public SubCategory Respond(IAddSubCategory.Request request)
    {
        Check.Positive(request.Order, () => ErrorMessagePersian.NotAllowed(Glossary.Order));
        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.Name));

        var duplicate = searchSubCategories.Respond(new ISearchSubCategories.Request
        {
            CategoryId = request.CategoryId,
            Name = request.Name.Clean(),
        }).Any();
        Check.False(duplicate, () => ErrorMessagePersian.Duplicate(Glossary.SubCategory));

        return db.Set<SubCategory>().Add(new SubCategory
        {
            CategoryId = request.CategoryId,
            Name = request.Name.Clean(),
            Description = request.Description,
            Order = request.Order,
            ImageUrl = request.ImageUrl,
            IsActive = request.IsActive
        }).Entity;
    }
}