using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MassTransit;

namespace Messaging;

public static class MessagingExtensions
{
    public static IServiceCollection AddCustomMassTransit(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRegistrationConfigurator> configureConsumers)
    {
        services.AddMassTransit(x =>
        {
            configureConsumers(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });

                // Retry Policy Global
                cfg.UseMessageRetry(r =>
                {
                    r.Exponential(5,
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromMinutes(5),
                        TimeSpan.FromSeconds(2));

                    r.Ignore<ArgumentNullException>();
                    r.Ignore<InvalidOperationException>();
                });

                // Dead Letter Queue
                // cfg.ReceiveEndpoint("order-error", e => {
                //     e.ConfigureError(settings => settings.UseEntityFramework());
                // });

                // Circuit Breaker
                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
