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
        Check.Given(request.Name, () => ErrorMessage.Unknown(Glossary.Name));
        Check.Given(request.Title, () => ErrorMessage.Unknown(Glossary.Title));
        Check.Given(request.Slug, () => ErrorMessage.Unknown(Glossary.Slug));

        Check.NationalCodeFormat(request.Username);
        Check.MobileFormat(request.Mobile, () => ErrorMessage.Invalid(Glossary.Mobile));

        Check.NotNull(request.CountyId, () => ErrorMessage.Unknown(Glossary.County));
        Check.Defined(request.Type, () => ErrorMessage.Unknown(Glossary.UserType));
        Check.Defined(request.BusinessType, () => ErrorMessage.Unknown(Glossary.BusinessType));

        var users = searchUsers.Respond();

        var mobileCodeDuplicate = users.Any(i => i.Mobile == request.Mobile);
        Check.False(mobileCodeDuplicate, () => ErrorMessage.Duplicate(Glossary.Mobile));

        var slugDuplicate = users.Any(i => i.Slug == request.Slug);
        Check.False(slugDuplicate, () => ErrorMessage.Duplicate(Glossary.Slug));

        var plainPassword = randomPasswordService.Generate12Chars();
        // todo : send password in sms for user

        var password = HashPassword.Hash(request.Username, plainPassword);

        var user= db.Set<User>().Add(new User
        {
            Name = request.Name.Clean(),
            Title = request.Title.Clean(),
            Description = request.Description.Clean(),
            Mobile = request.Mobile,
            Slug = request.Slug,
            Password = password,
            Type = request.Type,
            BusinessType=request.BusinessType,
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