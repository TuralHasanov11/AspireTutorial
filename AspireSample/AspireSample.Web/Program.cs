using AspireSample.Web.Services;
using OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(config => config.ValidateOnBuild = true);
builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient<ApiServiceClient>((sp, client) =>
{
    // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
    // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.

    var apiEndpoint = sp.GetRequiredService<IConfiguration>().GetValue<string>("ApiEndpoint");
    ArgumentException.ThrowIfNullOrEmpty(apiEndpoint, nameof(apiEndpoint));

    client.BaseAddress = new(apiEndpoint);
});

var oltpApiKey = builder.Configuration.GetValue<string>("OTLP_API_KEY");
builder.Services.Configure<OtlpExporterOptions>(o => o.Headers = $"x-otlp-api-key={oltpApiKey}");

builder.AddRedisOutputCache("cache");

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseOutputCache();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/apiservice", async (ApiServiceClient client) =>
{
    var result = await client.GetHomeAsync();
    return Results.Ok(result);
});

app.Run();
