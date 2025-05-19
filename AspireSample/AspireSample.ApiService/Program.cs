using AspireSample.ApiService.Data;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<CatalogDbContext>("catalog");

var oltpApiKey = builder.Configuration.GetValue<string>("OTLP_API_KEY");
builder.Services.Configure<OtlpExporterOptions>(o => o.Headers = $"x-otlp-api-key={oltpApiKey}");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        context.Database.EnsureCreated();
    }
}

app.MapDefaultEndpoints();

app.MapGet("/", () => "Api Service");

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

app.Run();