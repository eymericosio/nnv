using Destructurama;
using GraphQL;
using GraphQL.Types;
using Microsoft.OpenApi.Models;
using Movies.Core;
using Movies.GrainClients;
using Movies.Server.Gql;
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
	)
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies API", Version = "v1" });
});

builder.Services.AddGraphQL(builder => builder
	.AddSystemTextJson()
	.AddErrorInfoProvider(opt => opt.ExposeExceptionDetails = true)
);

builder.Services.AddScoped<ISchema, GqlTestSchema>();
builder.Services.AddScoped<GqlTestType>();
builder.Services.AddScoped<GqlTestQuery>();
builder.Services.AddScoped<GqlTestMutation>();

builder.Services.AddScoped<ISchema, GqlNotesSchema>();
builder.Services.AddScoped<GqlNoteType>();
builder.Services.AddScoped<GqlNotesQuery>();

builder.Services.AddGrainClients();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseGraphQLAltair();
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
app.UseGraphQL<ISchema>();

await app.RunAsync();
