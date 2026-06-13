using Services.Menus.Categories;

namespace Services.Menus.SubCategories;

public class CheckIsSubCategoryOwner(ISearchSubCategories searchSubCategories, IActorService actorService) : ICheckIsSubCategoryOwner
{
    public bool Respond(int id)
    {
        return searchSubCategories.Respond(new ISearchSubCategories.Request
        {
            Id = id,
            Category = new ISearchCategories.Request
            {
                Menu = new ISearchMenus.Request
                {
                    UserId = actorService.UserId
                }
            }
        }).Any();
    }
}