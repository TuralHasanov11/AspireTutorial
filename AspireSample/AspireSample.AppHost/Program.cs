using Aspire.Hosting.Lifecycle;
using AspireSample.AppHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

builder.Services.AddLifecycleHook<LifecycleLogger>();

var cache = builder.ExecutionContext.IsRunMode
    ? builder.AddRedis("cache")
        .WithDataVolume()
        .WithImageTag("latest")
        .WithClearCommand()
    : builder.AddConnectionString("cache");

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

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin(resource =>
    {
        resource.WithUrlForEndpoint("https", u => u.DisplayText = "PG Admin");
    });

var catalogDb = postgres.AddDatabase("catalog")
    .WithClearCommand();


var apiCacheInvalidationKey = builder.AddParameter("ApiCacheInvalidationKey", secret: true);

var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithUrlForEndpoint("https", url =>
    {
        url.DisplayText = "Scalar (HTTPS)";
        url.Url += "/scalar";
    })
    .WithEnvironment("API_CACHE_INVALIDATION_KEY", apiCacheInvalidationKey)
    .WithClearCache(apiCacheInvalidationKey)
    .WithReference(catalogDb)
    .WithReplicas(2)
    .WaitFor(catalogDb);

builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);


builder.Eventing.Subscribe<BeforeStartEvent>(
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("1. BeforeStartEvent");

        return Task.CompletedTask;
    });

builder.Eventing.Subscribe<AfterEndpointsAllocatedEvent>(
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("2. AfterEndpointsAllocatedEvent");

        return Task.CompletedTask;
    });

builder.Eventing.Subscribe<AfterResourcesCreatedEvent>(
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("3. AfterResourcesCreatedEvent");

        return Task.CompletedTask;
    });

builder.Build().Run();
