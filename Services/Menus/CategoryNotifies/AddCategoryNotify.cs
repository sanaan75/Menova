using Entities;
using Entities.Menus;

namespace Services.Menus.CategoryNotifies;

public class AddCategoryNotify(IDb db, ISearchCategoryNotifies searchCategoryNotifies) : IAddCategoryNotify
{
    public CategoryNotify Respond(IAddCategoryNotify.Request request)
    {
        Check.Positive(request.Order, () => ErrorMessagePersian.NotAllowed(Glossary.Order));
        Check.Given(request.Title, () => ErrorMessagePersian.Unknown(Glossary.Title));

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