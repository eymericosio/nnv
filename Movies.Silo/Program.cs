using Destructurama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Movies.Contracts;
using Movies.Core;
using Movies.Silo;
using Orleans.Runtime;
using Serilog;
using Serilog.Enrichers.Span;
using System.Reflection;
using System.Text.Json;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration(options => options.AddJsonFile("app-info.json"));

builder
	.UseSerilog((hostContext, loggerConfiguration) =>
	{
		var appInfo = new AppInfo(hostContext.Configuration);
		loggerConfiguration
			.ReadFrom.Configuration(hostContext.Configuration)
			.Destructure.UsingAttributes()
			.Enrich.FromLogContext()
			.Enrich.WithSpan()
			.Enrich.WithMachineName()
			.Enrich.WithDemystifiedStackTraces()
			.WithAppInfo(appInfo)
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
