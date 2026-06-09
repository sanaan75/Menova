namespace Entities.Plans;

public class Plan : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int MaxItems { get; set; }
    public int MaxImagePerItem { get; set; }
    public int Price { get; set; }
    public bool IsActive { get; set; }
    
    public ICollection<UserPlan>  UserPlans { get; set; }
}