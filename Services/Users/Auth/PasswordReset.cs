using Entities;
using Entities.Users;

namespace Services.Users.Auth;

public class PasswordReset(IDb db, ISearchPasswordResetTokens searchPasswordResetTokens, ITimeService timeService) : IPasswordReset
{
    public void Respond(IPasswordReset.Request request)
    {
        var now = timeService.Now;

        var token = searchPasswordResetTokens.Respond(db.Set<PasswordResetToken>(), new ISearchPasswordResetTokens.Request
        {
            UserId = request.UserId,
            Token = request.Token,
            Used = false
        }).SingleOrDefault(i => i.ExpireDate > now);

        Check.NotNull(token, () => ErrorMessagePersian.NotFound(Glossary.Token));

        token.Used = true;
        
        // todo : create log for
    }
}