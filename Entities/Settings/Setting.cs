namespace Entities.Settings;

public class Setting : IEntity
{
    public int Id { get; set; }
    public int UserConfirmCodeExpire { get; set; }
    public int ResetPassExpire { get; set; }
    public int AllowedRequestsPerMinute { get; set; }
}