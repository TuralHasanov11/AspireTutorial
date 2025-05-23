using Aspire.Hosting;

namespace AspireSample.Tests;

public class CatalogServiceTests : IAsyncLifetime
{
    private readonly CancellationToken _cancellationToken;
    private DistributedApplication _app;
    private IResourceBuilder<ProjectResource> _apiService;

    public CatalogServiceTests()
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

        _apiService = appHost.CreateResourceBuilder<ProjectResource>("catalogapi");

        _app = await appHost.BuildAsync(_cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();
        GC.SuppressFinalize(this); // Ensures that the finalizer is not called for this object
    }

    [Fact]
    public async Task GetApiServiceHealthReturnsOkStatusCode()
    {
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
        await _app.StartAsync(_cancellationToken);

        // Act
        using var httpClient = _app.CreateHttpClient("catalogapi");

        await resourceNotificationService.WaitForResourceAsync(
            "catalogapi",
            KnownResourceStates.Running,
            _cancellationToken);

        var response = await httpClient.GetAsync("/alive", _cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

}
