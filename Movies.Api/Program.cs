using Destructurama;
using GraphQL;
using GraphQL.Types;
using Microsoft.OpenApi.Models;
using Movies.GrainClients;
using Movies.Api.Gql.App;
using Movies.Api.Infrastructure;
using Serilog;
using Serilog.Enrichers.Span;
using Microsoft.AspNetCore.Authorization;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
	.ReadFrom.Configuration(context.Configuration)
	.ReadFrom.Services(services)
	.Destructure.UsingAttributes()
	.Enrich.FromLogContext()
	.Enrich.WithSpan()
	.Enrich.WithMachineName()
	.Enrich.WithDemystifiedStackTraces()
	.WriteTo.Console())
;

builder.Host.UseOrleansClient(client =>
{
	if (builder.Environment.IsDevelopment())
		client.UseLocalhostClustering();
	else
		client.UseStaticClustering(new IPEndPoint(IPAddress.Parse(builder.Configuration.GetValue<string>("Silo:Address")), builder.Configuration.GetValue<int>("Silo:Port")));
})
	.ConfigureLogging(logging => logging.AddConsole());

builder.Services.AddAuthentication("access_token")
	.AddOAuth2Introspection("access_token", options =>
	{
		options.Authority = builder.Configuration.GetValue<string>("OpenIdConnect:Authority");
		options.ClientId = "api";
		options.ClientSecret = "secret";
		options.EnableCaching = true;
	});

builder.Services.AddAuthorization(options =>
{
	options.DefaultPolicy = new AuthorizationPolicyBuilder()
		.AddAuthenticationSchemes("access_token")
		.RequireAuthenticatedUser()
		.Build()
	;
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
		.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
		.AllowAnyMethod()
		.AllowAnyHeader()
		.AllowCredentials()
	)
);

builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCompression();
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllers();
builder.Services.Configure<RouteOptions>(options =>
{
	options.LowercaseQueryStrings = true;
	options.LowercaseUrls = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies API", Version = "v1" });
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
	{
		Name = "Authorization",
		Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Name = "Bearer",
				In = ParameterLocation.Header,
				Reference = new OpenApiReference
				{
					Id = "Bearer",
					Type = ReferenceType.SecurityScheme
				}
			},
			new List<string>()
		}
	});
});

builder.Services.AddGraphQL(b => b
	.AddSystemTextJson()
	.AddErrorInfoProvider(opt => opt.ExposeExceptionDetails = true)
	.AddUserContextBuilder(httpContext => new MyGraphQLUserContext(httpContext.User))
	.AddSelfActivatingSchema<GqlMovieSchema>()
	.AddAuthorizationRule()
	.ConfigureExecutionOptions(options =>
	{
		options.EnableMetrics = builder.Environment.IsDevelopment();
	})
);

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
app.MapControllers().RequireAuthorization();
app.UseGraphQL<ISchema>("/graphql", options =>
{
	options.AuthorizationRequired = true;
});

await app.RunAsync();
