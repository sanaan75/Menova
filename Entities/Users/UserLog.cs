using Entities.Logs;

namespace Entities.Users;

public class UserLog : BaseLog
{
    public int UserId { get; set; }
    public User User { get; set; }
}