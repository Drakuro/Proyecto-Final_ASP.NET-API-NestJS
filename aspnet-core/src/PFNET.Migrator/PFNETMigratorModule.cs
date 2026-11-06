using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using PFNET.Configuration;
using PFNET.EntityFrameworkCore;
using PFNET.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace PFNET.Migrator;

[DependsOn(typeof(PFNETEntityFrameworkModule))]
public class PFNETMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public PFNETMigratorModule(PFNETEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(PFNETMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            PFNETConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(PFNETMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
