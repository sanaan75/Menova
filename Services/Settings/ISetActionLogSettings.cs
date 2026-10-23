namespace Services.Settings;

public interface ISetActionLogSettings
{
    void Respond(Request request);

    class Request
    {
        public int ActionLogMaxCount { get; set; }
        public int ActionLogMaxDays { get; set; }
    }
}
