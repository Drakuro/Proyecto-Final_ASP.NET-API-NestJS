using Abp.Application.Services.Dto;

namespace PFNET.Videojuegos.Dto;

public class VideojuegoDto : EntityDto<int>
{
    public string NombreDelJuego { get; set; } = string.Empty;

    public string ImagenDelJuego { get; set; } = string.Empty;
}
