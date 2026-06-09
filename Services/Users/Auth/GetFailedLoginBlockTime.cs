namespace Services.Users.Auth;

public class GetFailedLoginBlockTime : IGetFailedLoginBlockTime
{
    public int Respond() => 5;
}