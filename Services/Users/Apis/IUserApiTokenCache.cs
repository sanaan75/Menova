namespace Services.Users.Apis;

public interface IUserApiTokenCache : ISingleInstance
{
    Model Get(Guid token);

    public class Model
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}