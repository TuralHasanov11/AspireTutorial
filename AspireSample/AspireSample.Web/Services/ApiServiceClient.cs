namespace AspireSample.Web.Services;

public class ApiServiceClient(HttpClient httpClient)
{
    public async Task<string> GetHomeAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetStringAsync("/api", cancellationToken: cancellationToken);
    }
}
