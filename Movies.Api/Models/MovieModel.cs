using Movies.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Movies.Api.Models;

public class MovieModel : MovieCreateModel
{
	public MovieModel(Movie movie)
		: base(movie)
	{
		Key = movie.Key;
	}

	public string Key { get; set; }
}

public partial class MovieCreateModel : MovieUpdateModel
{
	[JsonConstructor]
	public MovieCreateModel()
	{
	}

	public MovieCreateModel(Movie movie)
		: base(movie)
	{
	}

	public string? GenerateKey()
	{
		if (string.IsNullOrEmpty(Name))
			return null;
		return KeyRegex().Replace(Name.ToLowerInvariant().Replace(" ", "-"), "");
	}

	[GeneratedRegex("[^a-zA-Z0-9-]+", RegexOptions.Compiled)]
	private static partial Regex KeyRegex();
}

public class MovieUpdateModel
{
	[JsonConstructor]
	public MovieUpdateModel()
	{
	}

	public MovieUpdateModel(Movie movie)
	{
		Name = movie.Name;
		Description = movie.Description;
		Rate = movie.Rate / 10m;
		Length = movie.Length;
		Img = movie.Img;
		Genres = movie.Genres.ToList();
	}

	public Movie ToMovie(string key) => new()
	{
		Key = key,
		Name = Name,
		Description = Description,
		Rate = (byte)(Rate * 10),
		Length = Length,
		Img = Img,
		Genres = Genres.ToHashSet()
	};

	[Required, Length(1, 250)]
	public string Name { get; set; } = string.Empty;

	[Required, Length(1, 999)]
	public string Description { get; set; } = string.Empty;

	[Required, Range(0d, 10d)]
	public decimal Rate { get; set; } = 0;

	[Required, Length(1, 250)]
	public string Length { get; set; } = string.Empty;

	[Required, Length(1, 250)]
	public string Img { get; set; } = string.Empty;

	[Required, Length(1, 9)]
	public List<string> Genres { get; set; } = [];
}
