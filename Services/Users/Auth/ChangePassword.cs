using Entities;
using Entities.Users;
using Services.Logs;
using Services.Users.Logs;

namespace Services.Users.Auth;

public class ChangePassword(IDb db, IActorService actorService, IValidatePasswordPattern validatePasswordPattern, IObjectChangeComparer objectChangeComparer, IAddUserLog addUserLog) : IChangePassword
{
    public void Respond(int id, string password)
    {
        Check.True(actorService.IsAuthenticated && (actorService.IsAdmin || actorService.UserId == id), () => ErrorMessagePersian.NotAllowed(Glossary.ChangePassword));

        validatePasswordPattern.Respond(password);

        var user = db.Set<User>().SingleOrDefault(i => i.Id == id);
        Check.NotNull(user, () => MessageFactory.NotFound(Glossary.User));
        var hashedPass = HashPassword.Hash(user.Username, password);

        objectChangeComparer.Respond(user, new { Password = hashedPass }, Glossary.EditUser);

        user.Password = hashedPass;

        var note = objectChangeComparer.GetChangesTextAndReset();
        addUserLog.Respond(user.Id, note);
    }
}
