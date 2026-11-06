using Abp.Application.Services;
using PFNET.Sessions.Dto;
using System.Threading.Tasks;

namespace PFNET.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
