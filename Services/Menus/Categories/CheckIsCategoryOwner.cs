namespace Services.Menus.Categories;

public class CheckIsCategoryOwner(ISearchCategories searchCategories, IActorService actorService) : ICheckIsCategoryOwner
{
    public bool Respond(int id)
    {
        var request = new ISearchCategories.Request
        {
            Id = id,
        };

        if (!actorService.IsAdmin)
        {
            request.Menu = new ISearchMenus.Request
            {
                UserId = actorService.UserId
            };
        }

        return searchCategories.Respond(request).Any();
    }
}
