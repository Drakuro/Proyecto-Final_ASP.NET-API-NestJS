using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using PFNET.Videojuegos.Dto;
using System;
using System.Threading.Tasks;

namespace PFNET.Videojuegos;

public class VideojuegoAppService : AsyncCrudAppService<Videojuego, VideojuegoDto, int, PagedAndSortedResultRequestDto, CreateVideojuegoDto, UpdateVideojuegoDto>, IVideojuegoAppService
{
    public VideojuegoAppService(IRepository<Videojuego, int> repository)
        : base(repository)
    {
    }
}
