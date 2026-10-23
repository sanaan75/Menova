using Entities.Basics.Security;

namespace Web;

public sealed class HasPagePermissionAttribute(params Permission[] permissions) : Attribute
{
    public Permission[] Permissions { get; } = permissions;
}
