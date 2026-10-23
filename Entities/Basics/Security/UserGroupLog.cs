using Entities.Logs;

namespace Entities.Basics.Security;

public class UserGroupLog : BaseLog
{
    public int UserGroupId { get; set; }
    public UserGroup UserGroup { get; set; }
}