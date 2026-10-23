using Entities.Logs;

namespace Services.Logs;

public interface IAddLog
{
    void Respond<T>(T log, string note) where T : BaseLog;
}