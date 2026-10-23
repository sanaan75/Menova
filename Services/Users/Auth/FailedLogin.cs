using System.Collections.Concurrent;

namespace Services.Users.Auth;

public class FailedLogin : IFailedLogin
{
    private static readonly ConcurrentDictionary<string, (int Count, DateTime LastAttempt)> UserFailures = new();
    private static readonly ConcurrentDictionary<string, (int Count, DateTime LastAttempt)> IpFailures = new();

    private const int MaxAttempts = 5;
    private static readonly TimeSpan LockDuration = TimeSpan.FromHours(24);

    public void RecordFailure(string username, string ip)
    {
        Record(UserFailures, username);
        Record(IpFailures, ip);
    }

    public bool IsBlocked(string username, string ip) => IsLocked(UserFailures, username) || IsLocked(IpFailures, ip);

    private static void Record(ConcurrentDictionary<string, (int, DateTime)> store, string key)
    {
        store.AddOrUpdate(key,
            _ => (1, DateTime.UtcNow),
            (_, old) => old.Item2 < DateTime.UtcNow - LockDuration
                ? (1, DateTime.UtcNow)
                : (old.Item1 + 1, DateTime.UtcNow));
    }

    private static bool IsLocked(ConcurrentDictionary<string, (int Count, DateTime LastAttempt)> store, string key) =>
        store.TryGetValue(key, out var entry)
        && entry.Count >= MaxAttempts
        && entry.LastAttempt >= DateTime.UtcNow - LockDuration;
}