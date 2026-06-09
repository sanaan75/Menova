using Entities;
using Services;

namespace Web;

public interface IApiSessionService : ISingleInstance
{
    Actor Login(string username, string password);
    Actor Login(Guid token);
}