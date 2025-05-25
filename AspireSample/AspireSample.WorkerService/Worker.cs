using System.Diagnostics;
using AspireSample.Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AspireSample.WorkerService;

public class Worker(
    ILogger<Worker> logger,
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        logger.LogInformation("Starting database migration.");

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<CatalogDbContext>();

            if (dbContext is not null)
            {
                await RunMigrationAsync(dbContext, stoppingToken);
                await SeedDataAsync(dbContext, stoppingToken);

                logger.LogInformation("Database migration completed successfully.");
            }
            else
            {
                logger.LogWarning("CatalogDbContext is not registered in the service provider. Migration skipped.");
            }

        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            logger.LogError(ex, "An error occurred during database migration.");
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(CatalogDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            if (dbContext.Database.GetMigrations().Any())
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
        });
    }

    private static async Task SeedDataAsync(CatalogDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            // Seed data here.
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}
