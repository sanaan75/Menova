using Entities;
using Entities.Users;

namespace Services.Users.Auth;

public class ValidatePassword(IDb db) : IValidatePassword
{
    public void Respond(IValidatePassword.Request request)
    {
        var hashToCheck = HashPassword.Hash(request.Username, request.Password);
        var user = db.Set<User>().SingleOrDefault(i => i.Username == request.Username);

        Check.NotNull(user, () => MessageFactory.Wrong($"{Glossary.Username} یا {Glossary.Password}"));
        Check.True(user.Enabled, () => MessageFactory.NotAllowedBecause("ورود به سامانه", "غیرفعال بودن حساب کاربری"));
        Check.Equal(user.Password, hashToCheck, () => MessageFactory.Wrong($"{Glossary.Username} یا {Glossary.Password}"));
    }
}
