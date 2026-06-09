namespace Services.Settings;

public interface ISettingsCache
{
    public void Reset();

    public Model Get();
    
    public class Model
    {
        public int UserConfirmCodeExpire { get; set; }
        public int ResetPassExpire { get; set; }
        public int AllowedRequestsPerMinute { get; set; }
    }
}