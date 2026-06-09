namespace Services.Users.Auth;

public interface IGetFailedLoginBlockTime : ISingleInstance
{
    int Respond();
}