using Entities;
using Entities.Users;

namespace Services.Users.Auth;

public class ValidatePassword : IValidatePassword
{
    private readonly IDb _db;

    public ValidatePassword(IDb db)
    {
        _db = db;
    }

    public void Respond(IValidatePassword.Request request)
    {
        var hashToCheck = HashPassword.Hash(request.Username, request.Password);
        var user = _db.Set<User>().SingleOrDefault(i => i.Username == request.Username);

        Check.NotNull(user, () => ErrorMessagePersian.Wrong($"{Glossary.Username} یا {Glossary.Password}"));
        Check.True(user.Enabled, () => "کاربر فعال نیست");
        Check.Equal(user.Password, hashToCheck, () => ErrorMessagePersian.Wrong($"{Glossary.Username} یا {Glossary.Password}"));
    }
}