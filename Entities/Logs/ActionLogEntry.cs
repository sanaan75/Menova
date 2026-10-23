namespace Entities.Logs;

public class ActionLogEntry : IEntity
{
    public int Id { get; set; }
    public LogModule ModuleName { get; set; }
    public int ModuleEntityId { get; set; }
    public string EntityName { get; set; }
    public int EntityId { get; set; }
    public string ActionName { get; set; }
    public DateTime ActionDate { get; set; }
    public string UserId { get; set; }
    public string LogData { get; set; }
}