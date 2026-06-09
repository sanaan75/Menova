using Entities.Users;

namespace Services.Users.Apis;

public interface ISearchUserApiToken
{
    IQueryable<UserApiToken> Respond(Request request = null);

    class Request
    {
        public bool ApplyActor { get; set; }
        public bool OnlyNotExpired { get; set; }
        public int? UserId { get; set; }
        public string Keyword { get; set; }
        public (DateTime? Start, DateTime? End)? CreateDate { get; set; }
        public Guid? Token { get; set; }
    }
}