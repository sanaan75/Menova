using Entities.Menus;

namespace Services.Menus.SubCategories;

public interface IGetSubCategoryInfo
{
    SubCategory Respond(int id);
}