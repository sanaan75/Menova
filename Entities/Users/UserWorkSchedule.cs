namespace Entities.Users;

public class UserWorkSchedule : IEntity
{
    public int Id { get; set; }

    public User User { get; set; }
    public int UserId { get; set; }
    
    public int StartHour { get; set; }
    public DayOfWeek FromDayOfWeek { get; set; }

    public int EndHour { get; set; }
    public DayOfWeek ToDayOfWeek { get; set; }

    public int Order { get; set; }
}