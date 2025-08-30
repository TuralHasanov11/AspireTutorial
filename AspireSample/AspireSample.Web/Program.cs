using AspireSample.Web;
using AspireSample.Web.Identity;
using AspireSample.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using Scalar.AspNetCore;

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
            if (builder.Environment.IsDevelopment())
            {
                options.RequireHttpsMetadata = false;
                options.Authority = "http://localhost:8082/realms/AspireSample";
            }
            else
            {
                options.Authority = "https://your-keycloak-server.com/realms/MyRealm";

            }
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.Scope.Add("catalog:read-write");
            options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
            options.SaveTokens = true;
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddCascadingAuthenticationState();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOptions<OpenApiInfo>()
    .BindConfiguration("OpenApiInfo")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<OpenApiInfoTransformer>();
    options.AddDocumentTransformer<OAuth2SecuritySchemeTransformer>();
});

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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference(options => options
        .AddPreferredSecuritySchemes("OAuth2")
        .AddAuthorizationCodeFlow("OAuth2", flow =>
        {
            flow.ClientId = "AspireSampleWeb";
            //flow.ClientSecret = "scalar-demo-secret";
            flow.RedirectUri = "https://localhost:7260/signin-oidc";
            //flow.Pkce = Pkce.Sha256;
        })).AllowAnonymous();
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
