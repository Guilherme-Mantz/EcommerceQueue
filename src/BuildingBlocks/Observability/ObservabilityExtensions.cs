using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace Observability;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddCustomObservability(
        this IServiceCollection services,
        string serviceName)
    {
        // OpenTelemetry
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource(serviceName)
                    .AddSource("MassTransit");
            })
            .WithMetrics(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddPrometheusExporter();
            });

        // Serilog
        services.AddSerilog((serviceProvider, loggerConfiguration) =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            loggerConfiguration
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Service", serviceName)
                .Enrich.WithProperty("MachineName", System.Environment.MachineName);
        });

        services.AddSingleton(Log.Logger);

        return services;
    }
}