using Abp.Zero.EntityFrameworkCore;
using PFNET.Authorization.Roles;
using PFNET.Authorization.Users;
using PFNET.MultiTenancy;
using PFNET.Videojuegos;
using Microsoft.EntityFrameworkCore;

namespace PFNET.EntityFrameworkCore;

public class PFNETDbContext : AbpZeroDbContext<Tenant, Role, User, PFNETDbContext>
{
    /* Define a DbSet for each entity of the application */

    public DbSet<Videojuego> Videojuegos { get; set; }

    public PFNETDbContext(DbContextOptions<PFNETDbContext> options)
        : base(options)
    {
    }
}
