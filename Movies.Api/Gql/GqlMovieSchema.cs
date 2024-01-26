using GraphQL.Types;

namespace Movies.Api.Gql.App;

public class GqlMovieSchema : Schema
{
	public GqlMovieSchema(IServiceProvider provider)
		: base(provider)
	{
		Query = provider.GetRequiredService<GqlMovieQuery>();
		Mutation = provider.GetRequiredService<GqlMovieMutation>();
	}
}
