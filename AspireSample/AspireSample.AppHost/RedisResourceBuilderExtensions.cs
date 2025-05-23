using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace AspireSample.AppHost;

internal static class RedisResourceBuilderExtensions
{
    internal static void AddCacheEvents(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<IResourceWithConnectionString> cache)
    {
        builder.Eventing.Subscribe<ResourceReadyEvent>(
            cache.Resource,
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("3. ResourceReadyEvent");

                return Task.CompletedTask;
            });

        builder.Eventing.Subscribe<BeforeResourceStartedEvent>(
            cache.Resource,
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("2. BeforeResourceStartedEvent");

                return Task.CompletedTask;
            });

        builder.Eventing.Subscribe<ConnectionStringAvailableEvent>(
            cache.Resource,
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("1. ConnectionStringAvailableEvent");

                return Task.CompletedTask;
            });
    }

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
