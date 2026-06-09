
namespace Services.Users.Auth;

public interface IFailedLoginService : ISingleInstance
{
    void LogFailedLogin(string ipAddress);
    int GetFailedLoginAttempts(string ipAddress);
    bool IsIPBlocked(string ipAddress);
}