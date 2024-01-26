using GraphQL.Types;
using Movies.Contracts;
using Movies.Api.Models;
using System.Runtime.CompilerServices;

namespace Movies.Api.Gql;

public class GqlMovieType : ObjectGraphType<MovieModel>
{
	public GqlMovieType()
	{
		Name = "movie";
		this.Key();
		this.Name();
		this.Description();
		this.Rate();
		this.Length();
		this.Img();
		this.Genres();
	}
}
