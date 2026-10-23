namespace Services.Settings;

public interface ISettingsCache
{
    public void Reset();

    public Model Get();
    
    public class Model
    {
        public int AllowUserAccountNo { get; set; }
        public int UserConfirmCodeExpire { get; set; }
        public int ResetPassExpire { get; set; }
        public int AllowedRequestsPerMinute { get; set; }
        public int ActionLogMaxCount { get; set; }
        public int ActionLogMaxDays { get; set; }
        public int MaxFileSizeInMB { get; set; }
        public int MaxLengthOfPassword { get; set; }
        public int MinLengthOfPassword { get; set; }
        public bool PasswordContainCapitalLetters { get; set; }
        public bool PasswordContainSpecialLetters { get; set; }
        public bool PasswordContainNumbers { get; set; }
    }
}
