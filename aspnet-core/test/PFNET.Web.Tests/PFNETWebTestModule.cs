using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using PFNET.EntityFrameworkCore;
using PFNET.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace PFNET.Web.Tests;

[DependsOn(
    typeof(PFNETWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class PFNETWebTestModule : AbpModule
{
    public PFNETWebTestModule(PFNETEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(PFNETWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(PFNETWebMvcModule).Assembly);
    }
}