namespace Entities.Settings;

public class Setting : IEntity
{
    public int Id { get; set; }
    public int AllowUserAccountNo { get; set; }
    public int UserConfirmCodeExpire { get; set; }
    public int ResetPassExpire { get; set; }
    public int AllowedRequestsPerMinute { get; set; }
    public int ActionLogMaxCount { get; set; } = 300_000;
    public int ActionLogMaxDays { get; set; } = 45;
    
    public int MaxFileSizeInMB { get; set; }
    public int MaxLengthOfPassword { get; set; }
    public int MinLengthOfPassword { get; set; }
    public bool PasswordContainCapitalLetters { get; set; }
    public bool PasswordContainSpecialLetters { get; set; }
    public bool PasswordContainNumbers { get; set; }
}