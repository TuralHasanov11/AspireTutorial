using Aspire.Hosting.Lifecycle;
using AspireSample.AppHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

builder.Services.AddLifecycleHook<LifecycleLogger>();

var seq = builder.AddSeq("seq")
    .WithDataVolume()
    .ExcludeFromManifest()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("ACCEPT_EULA", "Y");

var cache = builder.ExecutionContext.IsRunMode
    ? builder.AddRedis("cache")
        .WithDataVolume()
        .WithPersistence(
            interval: TimeSpan.FromMinutes(5),
            keysChangedThreshold: 100)
        .WithImageTag("latest")
        .WithRedisInsight()
        .WithClearCommand()
    : builder.AddConnectionString("cache");

builder.AddCacheEvents(cache);

var messaging = builder.AddRabbitMQ("messaging");

var mongo = builder.AddMongoDB("mongo")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithMongoExpress();

var postgresPassword = builder.AddParameter("PostgresPassword", secret: true);
var postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithLifetime(ContainerLifetime.Persistent);

if (builder.Configuration.GetValue("UseVolumes", true))
{
    postgres.WithDataVolume()
        .WithPgAdmin(resource =>
        {
            resource.WithUrlForEndpoint("https", u => u.DisplayText = "PG Admin");
        });
}

var catalogDb = postgres.AddDatabase("catalogdb")
    .WithClearCommand();

var apiCacheInvalidationKey = builder.AddParameter("ApiCacheInvalidationKey", secret: true);

var catalogService = builder.AddProject<Projects.AspireSample_Catalog_Api>("catalogapi")
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
    .WaitFor(catalogDb)
    .WithReference(messaging)
    .WaitFor(messaging)
    .WithReference(seq)
    .WaitFor(seq);


builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(catalogService)
    .WaitFor(catalogService)
    .WithReference(seq)
    .WaitFor(seq);

builder.AddProject<Projects.AspireSample_WorkerService>("workerservice")
    .WithReference(catalogDb)
    .WaitFor(catalogDb)
    .WithReference(seq)
    .WaitFor(seq);

builder.SubsribeToHostEvents();

builder.Build().Run();
