using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace AspireSample.AppHost;

internal static class RedisResourceBuilderExtensions
{
    internal static IResourceBuilder<RedisResource> WithClearCommand(this IResourceBuilder<RedisResource> builder)
    {
        builder.WithCommand(
            name: "clear-cache",
            displayName: "Clear Cache",
            executeCommand: context => OnRunClearCacheCommandAsync(builder, context),
            new CommandOptions
            {
                UpdateState = OnUpdateResourceState,
                IconName = "AnimalRabbitOff",
                IconVariant = IconVariant.Filled
            });

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

    private static async Task<ExecuteCommandResult> OnRunClearCacheCommandAsync(
        IResourceBuilder<RedisResource> builder,
        ExecuteCommandContext context)
    {
        string connectionString = await builder.Resource.GetConnectionStringAsync(context.CancellationToken)
            ?? throw new InvalidOperationException($"Unable to get the '{context.ResourceName}' connection string.");

        await using var connection = ConnectionMultiplexer.Connect(connectionString);

        var database = connection.GetDatabase();

        await database.ExecuteAsync("FLUSHALL");

        return CommandResults.Success();
    }
}