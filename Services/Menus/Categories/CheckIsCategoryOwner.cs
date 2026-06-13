namespace Services.Menus.Categories;

public class CheckIsCategoryOwner(ISearchCategories searchCategories, IActorService actorService) : ICheckIsCategoryOwner
{
    public bool Respond(int id)
    {
        return searchCategories.Respond(new ISearchCategories.Request
        {
            Id = id,
            Menu = new ISearchMenus.Request
            {
                UserId = actorService.UserId
            }
        }).Any();
    }
}