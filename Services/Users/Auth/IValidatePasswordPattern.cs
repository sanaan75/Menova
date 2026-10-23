namespace Services.Users.Auth;

public interface IValidatePasswordPattern
{
    public void Respond(string password);
}