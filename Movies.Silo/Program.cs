using Destructurama;
using Movies.Core;
using Serilog;
using Serilog.Enrichers.Span;

var builder = WebApplication.CreateBuilder(args);

var shortEnvName = AppInfo.MapEnvironmentName(builder.Environment.EnvironmentName);
builder.Configuration
	.AddJsonFile($"appsettings.{shortEnvName}.json", optional: true)
	.AddJsonFile("app-info.json")
;

var appInfo = new AppInfo(builder.Configuration);
builder.Services.AddSingleton(appInfo);

builder.Host.UseSerilog((context, services, configuration) => configuration
	.ReadFrom.Configuration(context.Configuration)
	.ReadFrom.Services(services)
	.Destructure.UsingAttributes()
	.Enrich.FromLogContext()
	.Enrich.WithSpan()
	.Enrich.WithMachineName()
	.Enrich.WithDemystifiedStackTraces()
	.WithAppInfo(appInfo)
	.WriteTo.Console())
;

builder.Host.UseOrleans(silo =>
	{
		silo.UseLocalhostClustering()
			.AddMemoryGrainStorageAsDefault()
		;
	})
	.UseConsoleLifetime();

using var host = builder.Build();

await host.RunAsync();
