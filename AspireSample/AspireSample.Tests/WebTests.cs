namespace AspireSample.Tests;

public class WebTests
{
    private readonly CancellationToken _cancellationToken;

    public WebTests()
    {
        _cancellationToken = TestContext.Current.CancellationToken;

    }


    [Fact]
    public async Task GetApiServiceHealthReturnsOkStatusCode()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireSample_AppHost>(
            _cancellationToken);

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        await using var app = await appHost.BuildAsync(_cancellationToken);
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync(_cancellationToken);

        // Act
        var httpClient = app.CreateHttpClient("apiservice");
        await resourceNotificationService.WaitForResourceAsync("apiservice", KnownResourceStates.Running, _cancellationToken)
            .WaitAsync(TimeSpan.FromSeconds(1), _cancellationToken);
        var response = await httpClient.GetAsync("/alive", _cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetWebFrontendPageReturnsOkStatusCode()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireSample_AppHost>(_cancellationToken);
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        await using var app = await appHost.BuildAsync(_cancellationToken);
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync(_cancellationToken);

        // Act
        var httpClient = app.CreateHttpClient("webfrontend");
        await resourceNotificationService.WaitForResourceAsync("webfrontend", KnownResourceStates.Running, _cancellationToken)
            .WaitAsync(TimeSpan.FromSeconds(1), _cancellationToken);

        var response = await httpClient.GetAsync("/", _cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
