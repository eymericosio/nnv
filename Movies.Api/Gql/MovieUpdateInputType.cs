using GraphQL.Types;
using Movies.Api.Models;

namespace Movies.Api.Gql.App;

public class MovieUpdateInputType : InputObjectGraphType<MovieUpdateModel>
{
	public MovieUpdateInputType()
	{
		Name = "MovieUpdate";
		this.Name();
		this.Description();
		this.Rate();
		this.Length();
		this.Img();
		this.Genres();
	}
}