using AspireSample.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithImageTag("latest")
    .WithClearCommand();

//var cache = builder.ExecutionContext.IsRunMode
//    ? builder.AddRedis("cache")
//    : builder.AddConnectionString("cache");


var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin(resource =>
    {
        resource.WithUrlForEndpoint("https", u => u.DisplayText = "PG Admin");
    });

var catalogDb = postgres.AddDatabase("catalogdb")
    .WithClearCommand();


//var apiCacheInvalidationKey = builder.AddParameter("ApiCacheInvalidationKey", secret: true);

var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    //.WithEnvironment("ApiCacheInvalidationKey", apiCacheInvalidationKey)
    //.WithClearCache(apiCacheInvalidationKey)
    .WithReference(catalogDb)
    //.WithReplicas(3)
    .WaitFor(catalogDb);

builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
