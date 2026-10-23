using Entities;

namespace Services;

public interface IActorService
{
    void Set(Actor actor);
    Actor Get();

    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    int UserId { get; }
}
