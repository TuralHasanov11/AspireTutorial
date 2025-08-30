using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace AspireSample.Catalog.Api;

internal class OpenApiInfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var openApiInfo = context.ApplicationServices.GetRequiredService<IOptions<OpenApiInfo>>();

        document.Info = openApiInfo.Value;

        return Task.CompletedTask;
    }
}
