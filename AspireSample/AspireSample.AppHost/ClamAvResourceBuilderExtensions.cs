namespace AspireSample.AppHost;

public class ClamAvResource(string name) : ContainerResource(name), IResourceWithConnectionString
{

    internal const string PrimaryEndpointName = "tcp";
    private EndpointReference? _primaryEndpoint;

    public EndpointReference PrimaryEndpoint => _primaryEndpoint ??= new(this, PrimaryEndpointName);
    public ReferenceExpression ConnectionStringExpression =>
      ReferenceExpression.Create(
        $"tcp://{PrimaryEndpoint.Property(EndpointProperty.Host)}:{PrimaryEndpoint.Property(EndpointProperty.Port)}"
      );
}


public static class ClamAvResourceBuilderExtensions
{
    public static IResourceBuilder<ClamAvResource> AddClamAv(
      this IDistributedApplicationBuilder builder,
      string name,
      int? port = null)
    {
        var resource = new ClamAvResource(name);

        return builder.AddResource(resource)
          .WithImage("clamav/clamav")
          .WithImageRegistry("docker.io")
          .WithImageTag("latest")
          .WithEnvironment(
            "CLAMAV_NO_FRESHCLAMD",
            builder.ExecutionContext.IsRunMode.ToString())
          .WithEndpoint(
            port: port,
            name: ClamAvResource.PrimaryEndpointName,
            targetPort: 3310);
    }

}
