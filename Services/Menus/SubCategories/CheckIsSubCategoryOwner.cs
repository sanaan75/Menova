using Services.Menus.Categories;

namespace Services.Menus.SubCategories;

public class CheckIsSubCategoryOwner(ISearchSubCategories searchSubCategories, IActorService actorService) : ICheckIsSubCategoryOwner
{
    public bool Respond(int id)
    {
        var request = new ISearchSubCategories.Request
        {
            Id = id,
        };

        if (!actorService.IsAdmin)
        {
            request.Category = new ISearchCategories.Request
            {
                Menu = new ISearchMenus.Request
                {
                    UserId = actorService.UserId
                }
            };
        }

        return searchSubCategories.Respond(request).Any();
    }
}
