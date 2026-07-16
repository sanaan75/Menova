using Entities.Menus;

namespace Services.Menus.Categories;

public interface IGetCategoryInfo
{
    Category Respond(int id);
}