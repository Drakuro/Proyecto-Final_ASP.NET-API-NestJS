using Abp.Application.Services;
using PFNET.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace PFNET.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
