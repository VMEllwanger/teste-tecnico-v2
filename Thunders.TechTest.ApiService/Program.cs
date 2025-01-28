using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;
using System.Diagnostics.CodeAnalysis;
using Thunders.TechTest.ApiService.Configuration;
using Thunders.TechTest.ApiService.Data;
using Thunders.TechTest.ApiService.Handlers;
using Thunders.TechTest.ApiService.Logging;
using Thunders.TechTest.ApiService.Messaging;
using Thunders.TechTest.ApiService.Metrics;
using Thunders.TechTest.ApiService.Middleware;
using Thunders.TechTest.ApiService.Repository;
using Thunders.TechTest.ApiService.Repository.Interface;
using Thunders.TechTest.ApiService.Services;
using Thunders.TechTest.ApiService.Services.Interfaces;
using Thunders.TechTest.OutOfBox.Queues;


[ExcludeFromCodeCoverage]
internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();

		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "Thunders Tech Test API",
				Version = "v1",
				Description = "API para processamento de pedágios e geração de relatórios",
				Contact = new OpenApiContact
				{
					Name = "Thunders Tecnologia",
					Email = "contato@thunders.com.br"
				}
			});

			var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
			c.IncludeXmlComments(xmlPath);

			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = "JWT Authorization header using the Bearer scheme",
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer"
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
			});
		});


		builder.Services.AddOpenTelemetry()
			.ConfigureResource(resource => resource
				.AddService(serviceName: "Thunders.TechTest.ApiService"))
			.WithTracing(tracing => tracing
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddSource(ActivitySourceProvider.Source.Name)
				.AddOtlpExporter())
			.WithMetrics(metrics => metrics
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddMeter(BusinessMetrics.Meter.Name)
				.AddOtlpExporter());

		builder.Services.AddRebus(configure => configure
			.Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "thunders-tech-test"))
			.Routing(r => r.TypeBased()
				.Map<ProcessTollUsageMessage>("thunders-tech-test")
				.Map<GenerateReportMessage>("thunders-tech-test"))
			.Logging(l => l.Use(new Rebus.Logging.ConsoleLoggerFactory(true)))
			.Options(o =>
			{
				o.SetNumberOfWorkers(1);
				o.SetMaxParallelism(1);
			}));

		var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
		logger.LogInformation("Registrando handlers...");


		builder.Services.AddScoped<IHandleMessages<GenerateReportMessage>, GenerateReportsHandler>();

		logger.LogInformation("Handlers registrados com sucesso");

		builder.Services.AddDbContext<ApplicationDbContext>(options =>
			options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

		builder.Services.AddScoped<ITollUsageService, TollUsageService>();
		builder.Services.AddScoped<IReportService, ReportService>();
		builder.Services.AddScoped<IMessageSender, RebusMessageSender>();

		builder.Services.AddScoped<ITollUsageRepository, TollUsageRepository>();
		builder.Services.AddScoped<IHourlyCityReportRepository, HourlyCityReportRepository>();
		builder.Services.AddScoped<IMonthlyPlazaReportRepository, MonthlyPlazaReportRepository>();
		builder.Services.AddScoped<IVehicleTypeReportRepository, VehicleTypeReportRepository>();

		builder.Services.AddCors(options =>
		{
			options.AddDefaultPolicy(policy =>
			{
				policy.AllowAnyOrigin()
					  .AllowAnyHeader()
					  .AllowAnyMethod();
			});
		});

		var app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Thunders Tech Test API v1");
				c.RoutePrefix = string.Empty;
			});
		}

		app.UseRouting();

		app.UseCors();
		app.UseHttpsRedirection();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});

		app.UseMiddleware<ErrorHandlingMiddleware>();

		app.UseAuthorization();
		app.MapControllers();

		app.Run();
	}
}
