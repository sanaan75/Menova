namespace Entities.Users;

public class UserApiToken : IEntity
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public DateTime CreateDate { get; set; }
    public DateTime ExpireDate { get; set; }
    
    public Guid Token { get; set; }
    
    public string Host { get; set; }
    public string Note { get; set; }
}