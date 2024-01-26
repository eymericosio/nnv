using GraphQL.Types;
using Movies.Api.Models;

namespace Movies.Api.Gql;

public static class GqlMovieExtensions
{
	public static void Key<T>(this ComplexGraphType<T> type) where T : MovieCreateModel => type.Field(x => x.Key, nullable: false).Description("Unique key.");
	public static void Name<T>(this ComplexGraphType<T> type) where T : MovieUpdateModel => type.Field(x => x.Name, nullable: false).Description("Name.");
	public static void Description<T>(this ComplexGraphType<T> type) where T : MovieUpdateModel => type.Field(x => x.Description, nullable: false).Description("Description.");
	public static void Rate<T>(this ComplexGraphType<T> type) where T : MovieUpdateModel => type.Field(x => x.Rate, nullable: false).Description("Rate (0 to 10).");
	public static void Length<T>(this ComplexGraphType<T> type) where T : MovieUpdateModel => type.Field(x => x.Length, nullable: false).Description("Duration.");
	public static void Img<T>(this ComplexGraphType<T> type) where T : MovieUpdateModel => type.Field(x => x.Img, nullable: false).Description("Image.");
	public static void Genres<T>(this ComplexGraphType<T> type) where T : MovieUpdateModel => type.Field(x => x.Genres, nullable: false).Description("Genres.");
}
