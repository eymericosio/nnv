using Destructurama;
using Microsoft.OpenApi.Models;
using Movies.Core;
using Movies.GrainClients;
using Movies.Server.Gql.App;
using Movies.Server.Infrastructure;
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

builder.Host.UseOrleansClient(client =>
{
	client.UseLocalhostClustering();
})
	.ConfigureLogging(logging => logging.AddConsole());

builder.Services.AddCustomAuthentication();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
		.AllowAnyOrigin()
		.AllowAnyMethod()
		.AllowAnyHeader()
		.AllowCredentials()
	)
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies API", Version = "v1" });
});

builder.Services.AddGainClients();
builder.Services.AddAppGraphQL();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseGraphQL();
app.UseGraphQLPlayground();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.DisplayRequestDuration();
		options.EnableDeepLinking();
		options.EnablePersistAuthorization();
		options.EnableValidator();
	});
}

app.UseCors();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
