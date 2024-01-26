using GraphQL;
using GraphQL.Types;
using Movies.Contracts;
using Movies.Api.Infrastructure;
using Movies.Api.Models;

namespace Movies.Api.Gql.App;

public class GqlMovieMutation : ObjectGraphType
{
	public GqlMovieMutation(IMovieService service)
	{
		// TODO input validation, example Rate should be between 0 and 10
		// No easy way to do that directly via GQL as far as I know

		Field<GqlMovieType>("createMovie")
		  .Argument<NonNullGraphType<MovieCreateInputType>>("movie")
		  .ResolveAsync(async context =>
		  {
			  // to handle some authorization logic if need be
			  var userContext = context.UserContext as MyGraphQLUserContext;

			  var model = context.GetArgument<MovieCreateModel>("movie");
			  var movie = await service.Fetch(model.Key!);
			  if (movie.Name is not null)
				  throw new ExecutionError("Key must be unique");
			  movie = await service.Upsert(model.ToMovie(model.Key!));
			  return new MovieModel(movie);
		  });

		Field<GqlMovieType>("updateMovie")
			.Argument<NonNullGraphType<IdGraphType>>("key")
			.Argument<NonNullGraphType<MovieUpdateInputType>>("movie")
			.ResolveAsync(async context =>
			{
				var key = context.GetArgument<string>("key");
				var model = context.GetArgument<MovieUpdateModel>("movie");
				var movie = await service.Fetch(key);
				if (movie.Name is null || movie.IsDeleted)
					throw new ExecutionError("Invalid Key");
				movie = await service.Upsert(model.ToMovie(key));
				return new MovieModel(movie);
			});
		;
	}
}
