namespace Services.Users;

public interface ISignupUser
{
    Task Respond(Request request);

    class Request
    {
        public string NationalCode { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Password { get; set; }
    }
}