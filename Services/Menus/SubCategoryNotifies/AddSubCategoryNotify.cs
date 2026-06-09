using Entities;
using Entities.Menus;

namespace Services.Menus.SubCategoryNotifies;

public class AddSubCategoryNotify(IDb db, ISearchSubCategoryNotifies searchSubCategoryNotifies) : IAddSubCategoryNotify
{
    public SubCategoryNotify Respond(IAddSubCategoryNotify.Request request)
    {
        Check.Positive(request.Order, () => ErrorMessagePersian.NotAllowed(Glossary.Order));
        Check.Given(request.Title, () => ErrorMessagePersian.Unknown(Glossary.Title));

        var duplicate = searchSubCategoryNotifies.Respond(new ISearchSubCategoryNotifies.Request
        {
            SubCategoryId = request.SubCategoryId,
            Title = request.Title.Clean(),
        }).Any();
        Check.False(duplicate, () => ErrorMessagePersian.Duplicate(Glossary.MenuSubCategoryNotify));

        return db.Set<SubCategoryNotify>().Add(new SubCategoryNotify
        {
            SubCategoryId = request.SubCategoryId,
            Title = request.Title.Clean(),
            Description = request.Description,
            Order = request.Order,
            IsActive = request.IsActive
        }).Entity;
    }
}