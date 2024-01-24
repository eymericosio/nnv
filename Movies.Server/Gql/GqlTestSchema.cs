using GraphQL.Types;

namespace Movies.Server.Gql.App;

public class GqlTestSchema : Schema
{
	public GqlTestSchema(IServiceProvider provider)
		: base(provider)
	{
		Query = provider.GetRequiredService<GqlTestQuery>();
		Mutation = provider.GetRequiredService<GqlTestMutation>();
	}
}