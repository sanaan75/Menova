using Entities.Users;

namespace Services.Users.Auth;

public interface ISearchPasswordResetTokens
{
    public IQueryable<PasswordResetToken> Respond(Request request = null);
    public IQueryable<PasswordResetToken> Respond(IQueryable<PasswordResetToken> items,Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        public int? UserId { get; set; }
        public string Token { get; set; }
        public bool? Used { get; set; }
    }
}