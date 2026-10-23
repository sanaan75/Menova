using Entities.Users;

namespace Entities.Logs;

public abstract class BaseLog : IEntity
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public int DoerId { get; set; }
    public User Doer { get; set; }
    
    public string Note { get; set; }
}