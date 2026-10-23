using Entities;
using Entities.Menus;

using Services.Menus.Categories;

namespace Services.Menus.CategoryNotifies;

public class AddCategoryNotify(
    IDb db,
    ISearchCategoryNotifies searchCategoryNotifies,
    ICheckIsCategoryOwner checkIsCategoryOwner)
    : IAddCategoryNotify
{
    public CategoryNotify Respond(IAddCategoryNotify.Request request)
    {
        Check.Positive(request.Order, () => ErrorMessagePersian.NotAllowed(Glossary.Order));
        Check.Given(request.Title, () => ErrorMessagePersian.Unknown(Glossary.Title));
        Check.True(checkIsCategoryOwner.Respond(request.CategoryId),
            () => ErrorMessagePersian.NotAllowed($"{Glossary.Add} {Glossary.MenuCategoryNotify}"));

        var duplicate = searchCategoryNotifies.Respond(new ISearchCategoryNotifies.Request
        {
            CategoryId = request.CategoryId,
            Title = request.Title.Clean(),
        }).Any();
        Check.False(duplicate, () => ErrorMessagePersian.Duplicate(Glossary.MenuCategoryNotify));

        return db.Set<CategoryNotify>().Add(new CategoryNotify
        {
            CategoryId = request.CategoryId,
            Title = request.Title.Clean(),
            Description = request.Description,
            Order = request.Order,
            IsActive = request.IsActive
        }).Entity;
    }
}
