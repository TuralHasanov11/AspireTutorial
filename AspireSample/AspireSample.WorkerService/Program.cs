using AspireSample.Catalog.Infrastructure.Data;
using AspireSample.WorkerService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.Services.AddHostedService<Worker>();

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

var host = builder.Build();
host.Run();
