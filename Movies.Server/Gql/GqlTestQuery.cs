using GraphQL.Types;
using Movies.Contracts;

namespace Movies.Server.Gql.App;

public class GqlTestQuery : ObjectGraphType
{
	public GqlTestQuery(ISampleGrainClient sampleClient)
	{
		Field<GqlTestType>("sample")
			.Arguments(new QueryArgument<StringGraphType>
			{
				Name = "id"
			})
			.ResolveAsync(async ctx => await sampleClient.Get(ctx.Arguments["id"].Value.ToString()))
		;
	}
}
