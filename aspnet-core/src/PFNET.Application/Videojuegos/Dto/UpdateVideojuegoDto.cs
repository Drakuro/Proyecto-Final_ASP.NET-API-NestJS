using Abp.Application.Services.Dto;

namespace PFNET.Videojuegos.Dto;

public class UpdateVideojuegoDto : EntityDto<int>
{
    public string? NombreDelJuego { get; set; }

    public string? ImagenDelJuego { get; set; }
}
