using Aspire.Hosting;

namespace AspireSample.Tests;

public class WebTests : IAsyncLifetime
{
    private readonly CancellationToken _cancellationToken;
    private DistributedApplication _app;
    private IResourceBuilder<ProjectResource> _frontend;

    public WebTests()
    {
        _cancellationToken = TestContext.Current.CancellationToken;

    }

    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetWebFrontendPageReturnsOkStatusCode()
    {
        // Arrange
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
        await _app.StartAsync(_cancellationToken);

        // Act
        using var httpClient = _app.CreateHttpClient("webfrontend");

        await resourceNotificationService.WaitForResourceAsync(
            "webfrontend",
            KnownResourceStates.Running,
            _cancellationToken);

        var response = await httpClient.GetAsync("/", _cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public async ValueTask InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireSample_AppHost>(
            _cancellationToken);

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _frontend = appHost.CreateResourceBuilder<ProjectResource>("webfrontend");

        _app = await appHost.BuildAsync(_cancellationToken);
    }

    [Fact]
    public async Task WebResourceEnvVarsResolveToApiService()
    {
        // Arrange

        // Act
        var envVars = await _frontend.Resource.GetEnvironmentVariableValuesAsync(
            DistributedApplicationOperation.Publish);

        // Assert
        Assert.Contains(envVars, static (kvp) =>
        {
            var (key, value) = kvp;

            return key is "services__catalogapi__https__0"
                && value is "{catalogapi.bindings.https.url}";
        });
    }
}
