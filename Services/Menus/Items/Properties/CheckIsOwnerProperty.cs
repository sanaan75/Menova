namespace Services.Menus.Items.Properties;

public class CheckIsOwnerProperty(ISearchMenuItemProperty searchMenuItemProperty, IActorService actorService) : ICheckIsOwnerProperty
{
    public bool Respond(int id)
    {
        return searchMenuItemProperty.Respond(new ISearchMenuItemProperty.Request
        {
            Id = id,
            MenuItem = new ISearchMenuItems.Request
            {
                UserId = actorService.UserId
            }
        }).Any();
    }
}