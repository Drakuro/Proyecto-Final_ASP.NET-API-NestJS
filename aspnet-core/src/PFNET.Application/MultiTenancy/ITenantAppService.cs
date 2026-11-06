using Abp.Application.Services;
using PFNET.MultiTenancy.Dto;

namespace PFNET.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

