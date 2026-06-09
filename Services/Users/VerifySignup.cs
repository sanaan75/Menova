using Entities;
using Entities.Users;

namespace Services.Users;

public class VerifySignup(IDb db, ISearchUsers searchUsers, ITimeService timeService) : IVerifySignup
{
    public async Task Respond(string mobile, string confirmCode)
    {
        var user = db.Set<User>().FirstOrDefault(u => u.Mobile == mobile);
        Check.NotNull(user, () => ErrorMessagePersian.NotFound(Glossary.User));
        Check.False(user.Enabled, () => "کاربر فعال است");

        var userConfirms = db.Query<UserConfirmCode>().OrderByDescending(i => i.ExpireAt).FirstOrDefault();
        Check.True(timeService.Now < userConfirms.ExpireAt, () => ErrorMessagePersian.Wrong(Glossary.ConfirmCode));
        Check.True(confirmCode == userConfirms.Code, () => ErrorMessagePersian.Wrong(Glossary.ConfirmCode));
        
        user.Enabled = true;
    }
}