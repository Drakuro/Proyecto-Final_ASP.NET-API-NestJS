using Abp.Authorization;
using Abp.Runtime.Session;
using PFNET.Configuration.Dto;
using System.Threading.Tasks;

namespace PFNET.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : PFNETAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
