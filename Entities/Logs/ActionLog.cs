using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Logs;

public abstract class ActionLog
{
    [NotMapped]
    public abstract LogModule ModuleEntityProperty { get; }
}
