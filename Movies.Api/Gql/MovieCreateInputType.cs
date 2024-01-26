using GraphQL.Types;
using Movies.Api.Models;

namespace Movies.Api.Gql.App;

public class MovieCreateInputType : InputObjectGraphType<MovieCreateModel>
{
	public MovieCreateInputType()
	{
		Name = "MovieCreate";
		this.Key();
		this.Name();
		this.Description();
		this.Rate();
		this.Length();
		this.Img();
		this.Genres();
	}
}
