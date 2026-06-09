using Entities.Users;

namespace Services.Users;

public interface IAddUser
{
    User Respond(Request request);

    class Request
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public string Mobile { get; set; }
        public string Slug { get; set; }
        
        public string? Phone { get; set; }
        public string? InstagramLink { get; set; }
        public string? TelegramLink { get; set; }
        public UserType Type { get; set; }
        public BusinessType BusinessType { get; set; }
        public int CountyId { get; set; }
    }
}