using AspireSample.Web.Identity;
using AspireSample.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(config => config.ValidateOnBuild = true);
builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient<ApiServiceClient>((sp, client) =>
{
    var apiEndpoint = sp.GetRequiredService<IConfiguration>().GetValue<string>("CatalogApiEndpoint");
    ArgumentException.ThrowIfNullOrEmpty(apiEndpoint, nameof(apiEndpoint));

    client.BaseAddress = new(apiEndpoint);
}).AddHttpMessageHandler<AccessTokenDelegatingHandler>();

var oltpApiKey = builder.Configuration.GetValue<string>("OTLP_API_KEY");
builder.Services.Configure<OtlpExporterOptions>(o => o.Headers = $"x-otlp-api-key={oltpApiKey}");

builder.AddRedisOutputCache("cache");

builder.Services.AddHttpContextAccessor()
    .AddTransient<AccessTokenDelegatingHandler>();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddKeycloakOpenIdConnect(
        serviceName: "keycloak",
        realm: "AspireSample",
        OpenIdConnectDefaults.AuthenticationScheme,
        options =>
        {
            options.ClientId = "AspireSampleWeb";
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.Scope.Add("catalog:read-write");
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
            options.SaveTokens = true;
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddTransient<IAntiVirusService, AntiVirusService>();

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

app.MapIdentityEndpoints();
app.MapIdentityApiEndpoints();

app.MapGet("/apiservice", async (ApiServiceClient client) =>
{
    var result = await client.GetHomeAsync();
    return Results.Ok(result);
});

app.Run();
