namespace Entities.Users;

public class PasswordResetToken : IEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public string Token { get; set; }

    public DateTime ExpireDate { get; set; }

    public bool Used { get; set; }
}