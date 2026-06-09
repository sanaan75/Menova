using Entities.Users;

namespace Services.Users;

public interface ISearchUsers
{
    IQueryable<User> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        public string Username { get; set; }
        public string LicenseId { get; set; }

        public UserType? Type { get; set; }
        public BusinessType? BusinessType { get; set; }
        public int? CountyId { get; set; }
        public bool? Enabled { get; set; }
    }
}