using AspireSample.Catalog.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(config => config.ValidateOnBuild = true);
builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddRedisDistributedCache("cache");

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<CatalogDbContext>(
    "catalogdb",
    settings =>
    {
        settings.CommandTimeout = 30;
    },
    options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
    });

//builder.AddMongoDBClient("mongo");

var oltpApiKey = builder.Configuration.GetValue<string>("OTLP_API_KEY");
builder.Services.Configure<OtlpExporterOptions>(o => o.Headers = $"x-otlp-api-key={oltpApiKey}");

builder.AddSeqEndpoint(connectionName: "seq");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRequestTimeouts();

app.UseOutputCache();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Catalog Api");

app.MapPost("/cache/invalidate", static async (
    [FromHeader(Name = "X-CacheInvalidation-Key")] string? header,
    IConfiguration config) =>
{
    var hasValidHeader = config.GetValue<string>("API_CACHE_INVALIDATION_KEY") is string key
        && header == key;

    if (!hasValidHeader)
    {
        return Results.Unauthorized();
    }

    // clear cache logic here

    return Results.Ok();
});

// generate endpoint that will delete database
app.MapPost("/clear-db", async (CatalogDbContext dbContext) =>
{
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.MigrateAsync();
    return Results.Ok("Catalog database cleared.");
});

app.Run();
