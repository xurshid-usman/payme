using Microsoft.EntityFrameworkCore;
using Payme.Data;

namespace Payme.Configurations;

public static class DbConfig
{
    public static void AddDbServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        var assemblyName = typeof(AppDbContext).Assembly.FullName;

        builder.Services.AddDbContext<AppDbContext>((provider, options) =>
        {
            options.UseNpgsql(connectionString,
                optionsBuilder => optionsBuilder
                    .MigrationsAssembly(assemblyName)
                    .MigrationsHistoryTable("__EFMigrationsHistory", "migration"))
                    .UseSnakeCaseNamingConvention();
        });

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}