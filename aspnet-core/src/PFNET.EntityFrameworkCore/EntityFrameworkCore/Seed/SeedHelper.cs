using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using PFNET.EntityFrameworkCore.Seed.Host;
using PFNET.EntityFrameworkCore.Seed.Tenants;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Transactions;

namespace PFNET.EntityFrameworkCore.Seed;

public static class SeedHelper
{
    public static void SeedHostDb(IIocResolver iocResolver)
    {
        WithDbContext<PFNETDbContext>(iocResolver, SeedHostDb);
    }

    public static void SeedHostDb(PFNETDbContext context)
    {
        context.SuppressAutoSetTenantId = true;

        // Host seed
        new InitialHostDbBuilder(context).Create();

        // Default tenant seed (in host database).
        new DefaultTenantBuilder(context).Create();
        new TenantRoleAndUserBuilder(context, 1).Create();

        // Game Vault seed (16 videojuegos) - idempotente
        if (!context.Videojuegos.Any())
        {
            context.Videojuegos.AddRange(
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "PUBG: Battlegrounds", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/578080/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Counter-Strike 2", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/730/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Dota 2", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/570/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Grand Theft Auto V", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/271590/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Elden Ring", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Baldur's Gate 3", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Cyberpunk 2077", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/1091500/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Red Dead Redemption 2", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/1174180/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "The Witcher 3: Wild Hunt", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/292030/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "God of War", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/1593500/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Sekiro: Shadows Die Twice", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/814380/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Hogwarts Legacy", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/990080/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Resident Evil 4", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/2050650/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Monster Hunter: World", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/582010/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Stardew Valley", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/413150/header.jpg" },
                new PFNET.Videojuegos.Videojuego { NombreDelJuego = "Hollow Knight", ImagenDelJuego = "https://cdn.cloudflare.steamstatic.com/steam/apps/367520/header.jpg" }
            );
            context.SaveChanges();
        }
    }

    private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext> contextAction)
        where TDbContext : DbContext
    {
        using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
        {
            using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
            {
                var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);

                contextAction(context);

                uow.Complete();
            }
        }
    }
}

