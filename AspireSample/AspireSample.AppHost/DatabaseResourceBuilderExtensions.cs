using Microsoft.Extensions.Diagnostics.HealthChecks;

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
        IResourceBuilder<PostgresDatabaseResource> _,
        ExecuteCommandContext context)
    {
        // send request to endpoint clear-db of CatalogApi resource

        var httpClient = context.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("catalogapi");

        using var request = new HttpRequestMessage(HttpMethod.Post, "/clear-db");

        var response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return new ExecuteCommandResult { Success = true };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync(context.CancellationToken);
            return new ExecuteCommandResult { Success = false, ErrorMessage = errorMessage };
        }
    }
}
