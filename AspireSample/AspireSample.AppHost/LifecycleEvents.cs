using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspireSample.AppHost;

internal static class LifecycleEvents
{
    internal static void SubsribeToHostEvents(this IDistributedApplicationBuilder builder)
    {
        builder.Eventing.Subscribe<BeforeStartEvent>(
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("1. BeforeStartEvent");

                return Task.CompletedTask;
            });

        builder.Eventing.Subscribe<AfterEndpointsAllocatedEvent>(
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("2. AfterEndpointsAllocatedEvent");

                return Task.CompletedTask;
            });

        builder.Eventing.Subscribe<AfterResourcesCreatedEvent>(
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("3. AfterResourcesCreatedEvent");

                return Task.CompletedTask;
            });

        builder.Eventing.Subscribe<ResourceReadyEvent>(
           static (@event, cancellationToken) =>
           {
               var logger = @event.Services.GetRequiredService<ILogger<Program>>();

               logger.LogInformation("4. ResourceReadyEvent");

               return Task.CompletedTask;
           });
    }
}
