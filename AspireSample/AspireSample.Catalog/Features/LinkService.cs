namespace AspireSample.Catalog.Api.Features;

public class LinkService(LinkGenerator linkGenerator, IHttpContextAccessor httpContext) : ILinkService
{
    public Link Generate(string endpointName, object? routeValues, string rel, string method)
    {
        return new Link(
            linkGenerator.GetUriByName(httpContext.HttpContext!, endpointName, routeValues) ?? string.Empty,
            method
        );
    }
}
