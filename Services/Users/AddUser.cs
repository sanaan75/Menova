using Entities;
using Entities.Users;
using Services.Menus;
using Services.Security;

namespace Services.Users;

public class AddUser(
    IDb db,
    ISearchUsers searchUsers,
    IAddMenu addMenu,
    IRandomPasswordService randomPasswordService)
    : IAddUser
{
    public User Respond(IAddUser.Request request)
    {
        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.Name));
        Check.Given(request.Title, () => ErrorMessagePersian.Unknown(Glossary.Title));
        Check.Given(request.Slug, () => ErrorMessagePersian.Unknown(Glossary.Slug));

        Check.Given(request.Username, () => ErrorMessagePersian.Unknown(Glossary.Username));
        Check.True(request.Username.Length >= 10, () => ErrorMessagePersian.NotAllowed("طول نام کاربری"));

        //Check.NationalCodeFormat(request.Username);
        Check.MobileFormat(request.Mobile, () => ErrorMessagePersian.Invalid(Glossary.Mobile));

        Check.NotNull(request.CountyId, () => ErrorMessagePersian.Unknown(Glossary.County));
        Check.Defined(request.Type, () => ErrorMessagePersian.Unknown(Glossary.UserType));
        Check.Defined(request.BusinessType, () => ErrorMessagePersian.Unknown(Glossary.BusinessType));

        var users = searchUsers.Respond();

        var mobileCodeDuplicate = users.Any(i => i.Mobile == request.Mobile);
        Check.False(mobileCodeDuplicate, () => ErrorMessagePersian.Duplicate(Glossary.Mobile));

        var slugDuplicate = users.Any(i => i.Slug == request.Slug);
        Check.False(slugDuplicate, () => ErrorMessagePersian.Duplicate(Glossary.Slug));

        var plainPassword = randomPasswordService.Generate12Chars();
        // todo : send password in sms for user

        var password = HashPassword.Hash(request.Username, plainPassword);

        var user = db.Set<User>().Add(new User
        {
            Name = request.Name.Clean(),
            Title = request.Title.Clean(),
            Description = request.Description.Clean(),
            Mobile = request.Mobile,
            Slug = request.Slug,
            Username = request.Username,
            Password = password,
            Type = request.Type,
            BusinessType = request.BusinessType,
            CountyId = request.CountyId,
            Phone = request.Phone,
            InstagramLink = request.InstagramLink,
            TelegramLink = request.TelegramLink,
            Enabled = true
        }).Entity;

        addMenu.Respond(new IAddMenu.Request
        {
            User = user,
            Name = "",
            Description = ""
        });

        return user;
    }
}