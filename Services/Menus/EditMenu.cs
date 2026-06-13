using Entities;
using Entities.Menus;

namespace Services.Menus;

public class EditMenu(IDb db, IActorService actorService) : IEditMenu
{
    public void Respond(IEditMenu.Request request)
    {
        var menu = db.Set<Menu>().GetById(request.Id);
        Check.True(menu.UserId == actorService.UserId, () => ErrorMessagePersian.NotAllowed(Glossary.Edit));

        menu.Name = request.Name;
        menu.Description = request.Description;
    }
}