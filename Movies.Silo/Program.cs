using Destructurama;
using Microsoft.Extensions.Hosting;
using Movies.Silo;
using Serilog;
using Serilog.Enrichers.Span;

var builder = Host.CreateDefaultBuilder(args);

builder
	.UseSerilog((hostContext, loggerConfiguration) =>
	{
		loggerConfiguration
			.ReadFrom.Configuration(hostContext.Configuration)
			.Destructure.UsingAttributes()
			.Enrich.FromLogContext()
			.Enrich.WithSpan()
			.Enrich.WithMachineName()
			.Enrich.WithDemystifiedStackTraces()
			.WriteTo.Console()
		;
	});

builder.UseOrleans(silo =>
	{
		silo.UseLocalhostClustering()
			.AddMemoryGrainStorage("memoryStorage")
			.AddStartupTask<SeedStorageTask>();
		;
	})
	.UseConsoleLifetime();

builder.ConfigureServices(services =>
{
	// setup potential local services
});

using var host = builder.Build();
await host.RunAsync();
