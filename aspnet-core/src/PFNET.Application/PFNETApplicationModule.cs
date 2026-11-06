using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using PFNET.Authorization;

namespace PFNET;

[DependsOn(
    typeof(PFNETCoreModule),
    typeof(AbpAutoMapperModule))]
public class PFNETApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<PFNETAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(PFNETApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}
