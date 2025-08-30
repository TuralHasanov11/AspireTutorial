namespace AspireSample.Catalog.Api.Features;

public interface ILinkService
{
    Link Generate(string endpointName, object? routeValues, string rel, string method);
}
