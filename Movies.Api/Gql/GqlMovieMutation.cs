﻿using GraphQL;
using GraphQL.Types;
using Movies.Contracts;
using Movies.Api.Models;

namespace Movies.Api.Gql.App;

public class GqlMovieMutation : ObjectGraphType
{
	public GqlMovieMutation(IMovieGrainClient client)
	{
		// TODO input validation, example Rate should be between 0 and 10
		// No easy way to do that directly via GQL as far as I know

		Field<GqlMovieType>("createMovie")
			.Argument<NonNullGraphType<MovieCreateInputType>>("movie")
			.ResolveAsync(async context =>
			{
				var model = context.GetArgument<MovieCreateModel>("movie");
				var key = model.GenerateKey() ?? throw new ExecutionError("Invalid key");
				var movie = await client.Fetch(key);
				if (movie is not null)
					throw new ExecutionError("Key must be unique");
				movie = await client.Upsert(model.ToMovie(key));
				return new MovieModel(movie);
			});

		Field<GqlMovieType>("updateMovie")
			.Argument<NonNullGraphType<IdGraphType>>("key")
			.Argument<NonNullGraphType<MovieUpdateInputType>>("movie")
			.ResolveAsync(async context =>
			{
				var key = context.GetArgument<string>("key");
				var model = context.GetArgument<MovieUpdateModel>("movie");
				var movie = await client.Fetch(key) ?? throw new ExecutionError("Invalid Key");
				movie = await client.Upsert(model.ToMovie(key));
				return new MovieModel(movie);
			});
		;

		Field<GqlMovieType>("deleteMovie")
			.Argument<NonNullGraphType<IdGraphType>>("key")
			.ResolveAsync(async context =>
			{
				var key = context.GetArgument<string>("key");
				var movie = await client.Delete(key) ?? throw new ExecutionError("Invalid Key");
				return new MovieModel(movie);
			});
		;
	}
}
