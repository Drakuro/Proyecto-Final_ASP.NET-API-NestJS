using Abp.AutoMapper;
using AutoMapper;
using PFNET.Videojuegos;
using PFNET.Videojuegos.Dto;

namespace PFNET.Videojuegos;

public class VideojuegoMapProfile : Profile
{
    public VideojuegoMapProfile()
    {
        CreateMap<Videojuego, VideojuegoDto>();
        CreateMap<CreateVideojuegoDto, Videojuego>();
        CreateMap<UpdateVideojuegoDto, Videojuego>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
