using GraphQL;
using GraphQL.Types;
using Movies.Contracts;
using Movies.Api.Models;

namespace Movies.Api.Gql.App;

public class GqlMovieQuery : ObjectGraphType
{
	public GqlMovieQuery(IMovieGrainClient client)
	{
		Field<GqlMovieType>("movie")
			.Argument<NonNullGraphType<IdGraphType>>("key")
			.ResolveAsync(async context =>
			{
				var key = context.GetArgument<string>("key");
				var movie = await client.Fetch(key) ?? throw new ExecutionError("Invalid Key");
				return new MovieModel(movie);
			});
		;

		Field<ListGraphType<GqlMovieType>>("movies")
			.Argument<StringGraphType>("text")
			.Argument<ListGraphType<StringGraphType>>("genres")
			.ResolveAsync(async context =>
			{
				var text = context.GetArgument<string?>("text");
				var genres = context.GetArgument<IEnumerable<string>?>("genres");
				var movies = await client.List(text, genres);
				return movies.Select(m => new MovieModel(m));
			});

		Field<ListGraphType<GqlMovieType>>("movies_top_rated")
			.ResolveAsync(async context =>
			{
				var movies = await client.TopRated();
				return movies.Select(m => new MovieModel(m));
			});
	}
}
