using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace PFNET.EntityFrameworkCore;

public static class PFNETDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<PFNETDbContext> builder, string connectionString)
    {
        builder.UseSqlite(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<PFNETDbContext> builder, DbConnection connection)
    {
        builder.UseSqlite(connection);
    }
}
