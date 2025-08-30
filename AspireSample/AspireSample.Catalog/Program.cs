using System.IdentityModel.Tokens.Jwt;
using AspireSample.Catalog.Api.Features;
using AspireSample.Catalog.Api.Features.Products;
using AspireSample.Catalog.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(config => config.ValidateOnBuild = true);
builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddRedisDistributedCache("cache");

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOptions<OpenApiInfo>()
    .BindConfiguration("OpenApiInfo")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<OpenApiInfoTransformer>();
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

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

builder.Services.AddMediator();

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(
        serviceName: "keycloak",
        realm: "AspireSample",
        options =>
        {
            options.Audience = "AspireSampleCatalogApi";
            if (builder.Environment.IsDevelopment())
            {
                options.RequireHttpsMetadata = false;
                options.Authority = "http://localhost:8082/realms/AspireSample";
            }
            else
            {
                options.Authority = "https://your-keycloak-server.com/realms/MyRealm";

            }

            options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidateLifetime = true;
        });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireAssertion(context =>
            context.User.HasClaim(c =>
                c.Type == "scope" && c.Value.Split(' ').Contains("catalog:read-write")))
        .Build();

    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILinkService, LinkService>();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseHttpLogging();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference().AllowAnonymous();
}

app.UseRequestTimeouts();

app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

app.MapGet("/api", () => "Catalog Api");

app.MapPost("/cache/invalidate", static (
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
})
    .WithSummary("Invalidate API Cache")
    .WithDescription("Invalidates the API cache. Requires a valid X-CacheInvalidation-Key header.")
    .WithTags("cache")
    .WithName("InvalidateApiCache")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

// generate endpoint that will delete database
app.MapPost("/clear-db", async (CatalogDbContext dbContext) =>
{
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.MigrateAsync();
    return Results.Ok("Catalog database cleared.");
});

app.MapPost("idempotency", ([FromHeader(Name = "X-Idempotency-Key")] string requestId) =>
{
    Console.WriteLine($"Received idempotency request with ID: {requestId}");
    return Results.Ok();
});

// Register Product endpoints
app.MapProductEndpoints();

await app.RunAsync();
