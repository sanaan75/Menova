using Entities;
using Entities.Menus;

namespace Services.Menus.Categories;

public class AddCategory(IDb db, ISearchCategories searchCategories) : IAddCategory
{
    public Category Respond(IAddCategory.Request request)
    {
        Check.Positive(request.MenuId, () => Glossary.Menu);
        Check.Given(request.Name, () => Glossary.Category);

        var duplicate = searchCategories.Respond(new ISearchCategories.Request
        {
            MenuId = request.MenuId,
            Name = request.Name.Clean()
        }).Any();
        Check.False(duplicate, () => ErrorMessage.Duplicate(Glossary.Category));

        return db.Set<Category>().Add(new Category
        {
            MenuId = request.MenuId,
            Name = request.Name.Clean(),
            Description = request.Description,
            Order = request.Order,
            IsActive = true
        }).Entity;
    }
}