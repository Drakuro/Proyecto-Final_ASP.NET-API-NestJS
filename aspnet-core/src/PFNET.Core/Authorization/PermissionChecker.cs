using Abp.Authorization;
using PFNET.Authorization.Roles;
using PFNET.Authorization.Users;

namespace PFNET.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
