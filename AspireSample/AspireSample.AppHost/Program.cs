using Aspire.Hosting.Lifecycle;

var builder = DistributedApplication.CreateBuilder(args);

builder.Services.AddLifecycleHook<LifecycleLogger>();

var username = builder.AddParameter("KeycloakUsername");
var password = builder.AddParameter("KeycloakPassword", secret: true);
var keycloak = builder.AddKeycloak("keycloak", 8080, username, password)
    .WithDataVolume()
    //.WithRealmImport("./Realms");
    .WithLifetime(ContainerLifetime.Persistent)
    .WithExternalHttpEndpoints();

//var ollama = builder.AddOllama("ollama")
//    .WithOpenWebUI();
//var gemma3_1b = ollama.AddModel("gemma3:1b");

#region Seq
var seq = builder.AddSeq("seq")
    .WithDataVolume()
    .ExcludeFromManifest()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("ACCEPT_EULA", "Y");
#endregion

//var av = builder.AddClamAv("antivirus");

#region Redis
var cache = builder.ExecutionContext.IsRunMode
    ? builder.AddRedis("cache")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume()
        .WithPersistence(
            interval: TimeSpan.FromMinutes(5),
            keysChangedThreshold: 100)
        //.WithImageTag("latest")
        .WithRedisInsight()
        .WithClearCommand()
    : builder.AddConnectionString("cache");

builder.AddCacheEvents(cache);
#endregion

#region RabbitMQ
var messaging = builder.AddRabbitMQ("messaging")
    .WithLifetime(ContainerLifetime.Persistent);
#endregion

#region MongoDB
var mongo = builder.AddMongoDB("mongo")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithMongoExpress();
#endregion

#region PostgresSQL
var postgresPassword = builder.AddParameter("PostgresPassword", secret: true);
var postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithPgAdmin(resource =>
    {
        resource.WithUrlForEndpoint("https", u => u.DisplayText = "PG Admin")
            //.WithImagePullPolicy(ImagePullPolicy.Always)
            .WithLifetime(ContainerLifetime.Persistent);
    });

var catalogDb = postgres.AddDatabase("catalogdb")
    .WithClearCommand();
#endregion

#region Catalog Service
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
    .WaitFor(seq)
    .WithReference(keycloak)
    .WaitFor(keycloak);
#endregion

#region WebApp
builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(catalogService)
    .WaitFor(catalogService)
    .WithReference(seq)
    .WaitFor(seq)
    .WithReference(keycloak)
    .WaitFor(keycloak);
#endregion

#region Worker Service
builder.AddProject<Projects.AspireSample_WorkerService>("workerservice")
    .WithReference(catalogDb)
    .WaitFor(catalogDb)
    .WithReference(seq)
    .WaitFor(seq);
#endregion

builder.SubsribeToHostEvents();


builder.AddNpmApp("reactappclient", "../reactapp.client", "dev")
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.AddNpmApp("vueappclient", "../vueapp.client", "dev")
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();


builder.AddProject<Projects.AiChatApp>("aichatapp")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(catalogService)
    .WaitFor(catalogService)
    .WithReference(seq)
    .WaitFor(seq)
    .WithReference(keycloak)
    .WaitFor(keycloak);


await builder.Build().RunAsync();
