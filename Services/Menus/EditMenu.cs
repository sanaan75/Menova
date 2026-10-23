using Entities;
using Entities.Menus;

namespace Services.Menus;

public class EditMenu(IDb db, IActorService actorService) : IEditMenu
{
    public void Respond(IEditMenu.Request request)
    {
        var menu = db.Set<Menu>().GetById(request.Id);
        Check.NotNull(menu, () => ErrorMessagePersian.NotFound(Glossary.Menu));

        Check.True(actorService.IsAdmin || menu.UserId == actorService.UserId, () => ErrorMessagePersian.NotAllowed(Glossary.Edit));

        menu.Name = request.Name;
        menu.Description = request.Description;
    }
}