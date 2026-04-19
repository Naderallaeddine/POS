using POS.Authorization;

namespace POS.Infrastructure.Identity;

public static class IdentityRoleSeed
{
    public const string AdminRoleId = "8f4a9ef9-0c89-43a5-9f3f-1c9a4a6df001";
    public const string ManagerRoleId = "8f4a9ef9-0c89-43a5-9f3f-1c9a4a6df003";
    public const string CashierRoleId = "8f4a9ef9-0c89-43a5-9f3f-1c9a4a6df002";

    public static (string Id, string Name)[] Roles => new[]
    {
        (AdminRoleId, AppRoles.Admin),
        (ManagerRoleId, AppRoles.Manager),
        (CashierRoleId, AppRoles.Cashier)
    };
}

