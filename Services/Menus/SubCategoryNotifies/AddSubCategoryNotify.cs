using Entities;
using Entities.Menus;

using Services.Menus.SubCategories;

namespace Services.Menus.SubCategoryNotifies;

public class AddSubCategoryNotify(
    IDb db,
    ISearchSubCategoryNotifies searchSubCategoryNotifies,
    ICheckIsSubCategoryOwner checkIsSubCategoryOwner)
    : IAddSubCategoryNotify
{
    public SubCategoryNotify Respond(IAddSubCategoryNotify.Request request)
    {
        Check.Positive(request.Order, () => ErrorMessagePersian.NotAllowed(Glossary.Order));
        Check.Given(request.Title, () => ErrorMessagePersian.Unknown(Glossary.Title));
        Check.True(checkIsSubCategoryOwner.Respond(request.SubCategoryId), () => ErrorMessagePersian.NotAllowed($"{Glossary.Add} {Glossary.MenuSubCategoryNotify}"));

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
