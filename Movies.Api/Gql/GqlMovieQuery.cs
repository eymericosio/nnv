using GraphQL;
using GraphQL.Types;
using Movies.Contracts;
using Movies.Api.Models;

namespace Movies.Api.Gql.App;

public class GqlMovieQuery : ObjectGraphType
{
	public GqlMovieQuery(IMovieService service)
	{
		Field<GqlMovieType>("movie")
			.Argument<NonNullGraphType<IdGraphType>>("key")
			.ResolveAsync(async context =>
			{
				var key = context.GetArgument<string>("key");
				var movie = await service.Fetch(key);
				if (movie.Name is null || movie.IsDeleted)
					throw new ExecutionError("Invalid Key");
				return new MovieModel(movie);
			});
		;

		Field<ListGraphType<GqlMovieType>>("movies")
			.Argument<StringGraphType>("search")
			.Argument<ListGraphType<StringGraphType>>("genres")
			.ResolveAsync(async context =>
			{
				var search = context.GetArgument<string?>("search");
				var genres = context.GetArgument<IEnumerable<string>?>("genres");
				var movies = await service.List(search, genres);
				return movies.Select(m => new MovieModel(m));
			});

		Field<ListGraphType<GqlMovieType>>("movies_top_rated")
			.ResolveAsync(async context =>
			{
				var movies = await service.TopRated();
				return movies.Select(m => new MovieModel(m));
			});
	}
}
