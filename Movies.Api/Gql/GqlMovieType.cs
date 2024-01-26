using GraphQL.Types;
using Movies.Api.Models;

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
