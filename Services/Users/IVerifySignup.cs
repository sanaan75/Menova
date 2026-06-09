namespace Services.Users;

public interface IVerifySignup
{
    Task Respond(string mobile , string confirmCode);
}