using Entities;
using Services.Users.Auth;

namespace Services.Users;

public class LoginApi(ISearchUsers searchUsers, ICreateActor createActor) : ILoginApi
{
    public Task<Actor> Respond(string username, string password)
    {
        try
        {
            var user = searchUsers.Respond(new ISearchUsers.Request
            {
                Username = username
            }).SingleOrDefault();

            Check.NotNull(user, () => MessageFactory.Wrong($"{Glossary.Username} یا {Glossary.Password}"));

            var hashedPass = HashPassword.Hash(username, password);
            Check.Equal(hashedPass, user.Password, () => MessageFactory.Wrong($"{Glossary.Username} یا {Glossary.Password}"));

            var actor = createActor.Respond(user.Username, user.Password, true);

            return Task.FromResult(actor);
        }
        catch (Exception exception)
        {
            return Task.FromException<Actor>(exception);
        }
    }
}