namespace Services.Users.Auth;

public interface IValidatePassword
{
    void Respond(Request request);

    public struct Request
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}