using Entities.Users;

namespace Entities.Plans;

public class UserPlan : IEntity
{
    public int Id { get; set; }
    
    public User User { get; set; }
    public int UserId { get; set; }
    
    public Plan Plan { get; set; }
    public int PlanId { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}