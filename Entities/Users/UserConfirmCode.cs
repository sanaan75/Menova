namespace Entities.Users;

public class UserConfirmCode : IEntity
{
    public int Id { get; set; }
    public string Code { get; set; }
    
    public DateTime ExpireAt { get; set; }
    
    public User User { get; set; }
    public int UserId { get; set; }
}