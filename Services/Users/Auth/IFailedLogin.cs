namespace Services.Users.Auth;

public interface IFailedLogin
{
    void RecordFailure(string username, string ip);
    bool IsBlocked(string username, string ip);
}