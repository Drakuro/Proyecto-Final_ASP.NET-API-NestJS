using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PFNET.Videojuegos.Dto;

namespace PFNET.Videojuegos;

public interface IVideojuegoAppService : IAsyncCrudAppService<VideojuegoDto, int, PagedAndSortedResultRequestDto, CreateVideojuegoDto, UpdateVideojuegoDto>
{
}
