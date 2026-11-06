using PFNET.Configuration.Dto;
using System.Threading.Tasks;

namespace PFNET.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
