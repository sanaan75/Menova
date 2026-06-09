using Entities;

namespace Services.Users;

public class SignupUser(
    ISearchUsers searchUsers,
    IAddUser addUser,
    IAddUserConfirmCode addUserConfirmCode)
    : ISignupUser
{
    public async Task Respond(ISignupUser.Request request)
    {
        Check.Given(request.NationalCode, () => ErrorMessagePersian.Unknown(Glossary.NationalCode));
        Check.NationalCodeFormat(request.NationalCode);

        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.Firstname));
        Check.Given(request.Title, () => ErrorMessagePersian.Unknown(Glossary.Lastname));
        Check.Given(request.Password, () => ErrorMessagePersian.Unknown(Glossary.Password));

        var normalizedNationalCode = request.NationalCode.Trim();

        var user = searchUsers.Respond(new ISearchUsers.Request
        {
            Username = normalizedNationalCode,
        }).FirstOrDefault();

        if (user is not null)
        {
            Check.False(user.Enabled, () => CommonMessages.AlreadyRegistered);
            addUserConfirmCode.Respond(user.Id);
        }

        user = addUser.Respond(new IAddUser.Request
        {
            Mobile = normalizedNationalCode,
            Name = request.Name,
            Title = request.Title,
            Username = normalizedNationalCode,
        });

        addUserConfirmCode.Respond(user);
    }
}