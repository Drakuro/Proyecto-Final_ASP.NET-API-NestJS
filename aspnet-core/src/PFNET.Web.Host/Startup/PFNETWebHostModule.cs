using Abp.Modules;
using Abp.Reflection.Extensions;
using PFNET.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace PFNET.Web.Host.Startup
{
    [DependsOn(
       typeof(PFNETWebCoreModule))]
    public class PFNETWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public PFNETWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PFNETWebHostModule).GetAssembly());
        }
    }
}
