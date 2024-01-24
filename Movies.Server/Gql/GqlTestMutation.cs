using GraphQL.Types;
using Movies.Contracts;

namespace Movies.Server.Gql.App;

public class GqlTestMutation : ObjectGraphType
{
	public GqlTestMutation(ISampleGrainClient sampleClient)
	{
		Field<GqlTestType>("setName")
			.Arguments(new QueryArgument<StringGraphType>
			{
				Name = "id"
			}, new QueryArgument<StringGraphType>
			{
				Name = "name"
			})
			.ResolveAsync(async ctx =>
			{
				await sampleClient.Set(ctx.Arguments["id"].ToString(), ctx.Arguments["name"].ToString());
				return await sampleClient.Get(ctx.Arguments["id"].ToString());
			})
		;
	}
}
