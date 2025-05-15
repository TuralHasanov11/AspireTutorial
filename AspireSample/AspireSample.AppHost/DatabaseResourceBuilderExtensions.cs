using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace AspireSample.AppHost;

internal static class DatabaseResourceBuilderExtensions
{
    internal static IResourceBuilder<PostgresDatabaseResource> WithClearCommand(
        this IResourceBuilder<PostgresDatabaseResource> builder)
    {
        builder.WithCommand(
            name: "clear-catalogdb",
            displayName: "Clear the Catalog Db",
            executeCommand: (context) => OnRunClearCatalogDbCommandAsync(builder, context),
            new CommandOptions
            {
                UpdateState = OnUpdateResourceState,
                IconName = "Database",
                IconVariant = IconVariant.Filled
            }
            );


        return builder;
    }

    private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Updating resource state: {ResourceSnapshot}",
                context.ResourceSnapshot);
        }

        return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
            ? ResourceCommandState.Enabled
            : ResourceCommandState.Disabled;
    }

    private static async Task<ExecuteCommandResult> OnRunClearCatalogDbCommandAsync(
        IResourceBuilder<PostgresDatabaseResource> builder,
        ExecuteCommandContext context)
    {
        throw new NotImplementedException();
    }
}