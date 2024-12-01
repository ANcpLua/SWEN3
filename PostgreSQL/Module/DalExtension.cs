using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PostgreSQL.Data;
using PostgreSQL.Persistence;

namespace PostgreSQL.Module;

public static class DalExtension
{
    public static void AddPostgreSqlServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaperlessDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IDocumentRepository, DocumentRepository>();
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PaperlessDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<PaperlessDbContext>>();

            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();

            logger.LogInformation("Seeding database...");
            await DatabaseSeeder.SeedDatabase(context);

            logger.LogInformation("Database setup completed.");
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<PaperlessDbContext>>();
            logger.LogError(ex, "Database setup failed.");
            throw;
        }
    }
}