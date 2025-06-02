using Ardalis.Result;
using nClam;

namespace AspireSample.Web.Services;

public interface IAntiVirusService
{
    Task<Result> Scan(IFormFile file);
}

public class AntiVirusService : IAntiVirusService
{
    private readonly string _clamConfig;
    private readonly Uri _clamUri;

    public AntiVirusService(IConfiguration configuration)
    {

        string? clamConfig = configuration.GetConnectionString("antivirus");

        ArgumentException.ThrowIfNullOrWhiteSpace(clamConfig);

        _clamConfig = clamConfig;
        _clamUri = new Uri(_clamConfig);

    }

    public async Task<Result> Scan(IFormFile file)
    {
        await using var stream = file.OpenReadStream();

        var clam = new ClamClient(_clamUri.Host, _clamUri.Port);
        var scanResult = await clam.SendAndScanFileAsync(stream);

        Result ErrorCode = scanResult.Result switch
        {
            ClamScanResults.VirusDetected => Result.Error($"Virus {scanResult?.InfectedFiles?.First().VirusName} detected"),
            ClamScanResults.Error => Result.Error($"Error with virus scanning.  Please try again"),
            ClamScanResults.Clean => Result.Success(),
            ClamScanResults.Unknown => Result.Error($"Unknown scan result: {scanResult.Result}"),
            _ => Result.Error($"Unknown scan result: {scanResult.Result}")
        };

        return ErrorCode;
    }
}
