namespace Services.Menus.Items.Properties;

public class CheckIsOwnerProperty(ISearchMenuItemProperty searchMenuItemProperty, IActorService actorService) : ICheckIsOwnerProperty
{
    public bool Respond(int id)
    {
        var request = new ISearchMenuItemProperty.Request
        {
            Id = id,
        };

        if (!actorService.IsAdmin)
        {
            request.MenuItem = new ISearchMenuItems.Request
            {
                UserId = actorService.UserId
            };
        }

        return searchMenuItemProperty.Respond(request).Any();
    }
}
