using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace PFNET.Controllers
{
    public abstract class PFNETControllerBase : AbpController
    {
        protected PFNETControllerBase()
        {
            LocalizationSourceName = PFNETConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
