using Entities.Basics.AdminLevels;

namespace Entities.Users;

public class User : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public string Username { get; set; }
    public string Mobile { get; set; }
    public string Slug { get; set; }

    public string Password { get; set; }
    
    public UserType Type { get; set; }
    public BusinessType BusinessType { get; set; }
    
    public County County { get; set; }
    public int CountyId { get; set; }
    
    public string? Phone { get; set; }
    public string? InstagramLink { get; set; }
    public string? TelegramLink { get; set; }

    public bool Enabled { get; set; }
    
    public ICollection<UserWorkSchedule> UserWorkSchedules { get; set; }
}