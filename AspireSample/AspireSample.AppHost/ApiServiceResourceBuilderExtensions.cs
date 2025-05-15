namespace AspireSample.AppHost;

internal static class ApiServiceResourceBuilderExtensions
{
    internal static IResourceBuilder<ProjectResource> WithClearCache(
        this IResourceBuilder<ProjectResource> builder,
        IResourceBuilder<ParameterResource> apiCacheInvalidationKey)
    {
        builder.WithHttpCommand(
            path: "/cache/invalidate",
            displayName: "Invalidate cache",
            commandOptions: new HttpCommandOptions()
            {
                Description = """
                    Invalidates the API cache. All cached values are cleared!
                    """,
                PrepareRequest = (context) =>
                {
                    string key = apiCacheInvalidationKey.Resource.Value;
                    context.Request.Headers.Add("X-CacheInvalidation-Key", key);

                    return Task.CompletedTask;
                },
                IconName = "DocumentLightning",
                IsHighlighted = true
            });

        return builder;
    }
}