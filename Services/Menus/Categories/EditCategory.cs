using Entities;
using Entities.Menus;

namespace Services.Menus.Categories;

public class EditCategory(IDb db, ISearchCategories searchCategories) : IEditCategory
{
    public void Respond(IEditCategory.Request request)
    {
        Check.Positive(request.Order, () => ErrorMessagePersian.NotAllowed(Glossary.Order));
        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.Name));

        var category = db.Set<Category>().GetById(request.Id);
        Check.NotNull(category, () => ErrorMessagePersian.NotFound(Glossary.Category));

        var duplicate = searchCategories.Respond(new ISearchCategories.Request
        {
            IgnoredId = request.Id,
            MenuId = category.MenuId,
            Name = request.Name.Clean(),
        }).Any();

        Check.False(duplicate, () => ErrorMessage.Duplicate(Glossary.Category));

        category.Name = request.Name.Clean();
        category.Description = request.Description;
        category.Order = request.Order;
        category.ImageUrl = request.ImageUrl;
    }
}