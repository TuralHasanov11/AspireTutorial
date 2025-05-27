namespace AspireSample.AppHost;

internal sealed class ExternalContainerResource(string name, string containerNameOrId) : Resource(name)
{
    public string ContainerNameOrId { get; } = containerNameOrId;
}

internal static class ExternalContainerResourceExtensions
{
    public static IResourceBuilder<ExternalContainerResource> AddExternalContainer(
        this IDistributedApplicationBuilder builder, string name, string containerNameOrId)
    {
        return builder.AddResource(new ExternalContainerResource(name, containerNameOrId))
            .WithInitialState(new CustomResourceSnapshot
            {
                ResourceType = "External container",
                State = "Starting",
                Properties = [new ResourcePropertySnapshot(CustomResourceKnownProperties.Source, "Custom")]
            })
            .ExcludeFromManifest();
    }
}
