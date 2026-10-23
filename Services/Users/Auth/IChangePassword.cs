namespace Services.Users.Auth;

public interface IChangePassword
{
    void Respond(int id, string password);
}