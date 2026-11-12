using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using PFNET.Authorization.Roles;
using PFNET.Authorization.Users;
using PFNET.MultiTenancy;
using Microsoft.AspNetCore.Identity;

namespace PFNET.Authorization;

public class LogInManager : AbpLogInManager<Tenant, Role, User>
{
    public LogInManager(
        UserManager userManager,
        IMultiTenancyConfig multiTenancyConfig,
        IRepository<Tenant> tenantRepository,
        IUnitOfWorkManager unitOfWorkManager,
        ISettingManager settingManager,
        IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
        IUserManagementConfig userManagementConfig,
        IIocResolver iocResolver,
        IPasswordHasher<User> passwordHasher,
        RoleManager roleManager,
        UserClaimsPrincipalFactory claimsPrincipalFactory)
        : base(
              userManager,
              multiTenancyConfig,
              tenantRepository,
              unitOfWorkManager,
              settingManager,
              userLoginAttemptRepository,
              userManagementConfig,
              iocResolver,
              passwordHasher,
              roleManager,
              claimsPrincipalFactory)
    {
    }

    // SQLite + ABP: SaveLoginAttemptAsync abre una segunda escritura dentro de
    // la misma unidad de trabajo y produce "database is locked" (SQLite no
    // soporta escrituras concurrentes). Se desactiva el registro de intentos de
    // login para que el flujo de autenticacion sea estable. El JWT se emite igual.
    public override System.Threading.Tasks.Task SaveLoginAttemptAsync(
        AbpLoginResult<Tenant, User> loginResult,
        string tenancyName,
        string userNameOrEmailAddress)
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }
}