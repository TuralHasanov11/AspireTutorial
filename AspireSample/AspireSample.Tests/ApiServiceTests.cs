using Aspire.Hosting;

namespace AspireSample.Tests;

public class ApiServiceTests : IAsyncLifetime
{
    private readonly CancellationToken _cancellationToken;
    private DistributedApplication _app;
    private IResourceBuilder<ProjectResource> _apiService;

    public ApiServiceTests()
    {
        _cancellationToken = TestContext.Current.CancellationToken;

    }

    public async ValueTask InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AspireSample_AppHost>(
                [
                    "UseVolumes=false",
                ],
                _cancellationToken);

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _apiService = appHost.CreateResourceBuilder<ProjectResource>("apiservice");

        _app = await appHost.BuildAsync(_cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();
    }

    [Fact]
    public async Task GetApiServiceHealthReturnsOkStatusCode()
    {
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
        await _app.StartAsync(_cancellationToken);

        // Act
        var httpClient = _app.CreateHttpClient("apiservice");

        await resourceNotificationService.WaitForResourceAsync(
            "apiservice",
            KnownResourceStates.Running,
            _cancellationToken);

        var response = await httpClient.GetAsync("/alive", _cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

}
