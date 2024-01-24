using GraphQL;
using Movies.Server.Gql.Types;

namespace Movies.Server.Gql.App;

public static class AppGqlExtensions
{
	public static void AddAppGraphQL(this IServiceCollection services)
	{
		services.AddGraphQL(builder => builder
			.AddSystemTextJson()
			.UseApolloTracing()
			.AddSchema<AppSchema>()
		);

		services.AddSingleton<AppGraphQuery>();
		services.AddSingleton<AppGraphMutation>();

		services.AddSingleton<SampleDataGraphType>();
	}
}
