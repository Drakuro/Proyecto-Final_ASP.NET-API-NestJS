using Abp.Domain.Entities;

namespace PFNET.Videojuegos;

public class Videojuego : Entity<int>
{
    public string NombreDelJuego { get; set; } = string.Empty;

    public string ImagenDelJuego { get; set; } = string.Empty;
}
