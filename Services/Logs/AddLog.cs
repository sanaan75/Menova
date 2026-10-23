using Entities.Logs;

namespace Services.Logs;

public class AddLog(IDb db, ITimeService timeService, IActorService actorService) : IAddLog
{
    public void Respond<T>(T log, string note) where T : BaseLog
    {
        if (log.Doer is null)
            log.DoerId = actorService.UserId;
        log.Date = timeService.Now;
        log.Note = note;

        db.Set<T>().Add(log);
    }
}
