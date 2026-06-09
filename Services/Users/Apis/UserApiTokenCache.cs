using Entities;
using Entities.Caches;

namespace Services.Users.Apis;

public class UserApiTokenCache : IUserApiTokenCache
{
    private readonly IContainer _mainContainer;
    private readonly IPartialCache<Guid, IUserApiTokenCache.Model> _modelById;

    public UserApiTokenCache(IContainer mainContainer, IPartialCache<Guid, IUserApiTokenCache.Model> modelById)
    {
        _mainContainer = mainContainer;
        _modelById = modelById;
        _modelById.Resolve = ResolveModelById;
    }

    private IUserApiTokenCache.Model ResolveModelById(Guid token)
    {
        using var container = _mainContainer.Get<IContainer>();
        var db = container.Get<ISearchUserApiToken>();

        var result = db.Respond(new ISearchUserApiToken.Request
            {
                Token = token
            })
            .Select(i => new IUserApiTokenCache.Model
            {
                Id = i.Id,
                UserId = i.UserId,
                ExpireDate = i.ExpireDate
            })
            .SingleOrDefault();

        Check.NotNull(result, () => ErrorMessage.Unknown($"{Glossary.Token} {token}"));

        return result;
    }

    public IUserApiTokenCache.Model Get(Guid token)
    {
        return _modelById.Get(token);
    }
}