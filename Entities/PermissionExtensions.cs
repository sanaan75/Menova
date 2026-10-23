using Entities.Basics.Security;

namespace Entities;

public static class PermissionExtensions
{
    public static bool Includes(this Permission granted, Permission requested)
    {
        var grantedCode = granted.ToString();
        var requestedCode = requested.ToString();

        return requestedCode.Equals(grantedCode, StringComparison.Ordinal) || requestedCode.StartsWith($"{grantedCode}_", StringComparison.Ordinal);
    }
}
