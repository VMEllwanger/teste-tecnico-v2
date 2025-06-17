using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Thunders.TechTest.ApiService.Configuration;
[ExcludeFromCodeCoverage]
public static class OpenTelemetryConfiguration
{
	public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
	{
		var serviceName = "Thunders.TechTest.ApiService";
		var serviceVersion = "1.0.0";

		services.AddOpenTelemetry()
			.ConfigureResource(resource => resource
				.AddService(serviceName: serviceName, serviceVersion: serviceVersion))
			.WithTracing(tracing => tracing
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddSource(serviceName)
				.AddConsoleExporter()
				.AddOtlpExporter(opts => opts.Endpoint = new Uri("http://localhost:4317")))
			.WithMetrics(metrics => metrics
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddRuntimeInstrumentation()
				.AddPrometheusExporter()
				.AddConsoleExporter()
				.AddOtlpExporter(opts => opts.Endpoint = new Uri("http://localhost:4317")));

		services.AddSingleton(new ActivitySource(serviceName));

		return services;
	}
}
