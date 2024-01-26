using Movies.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Movies.Api.Models;

public class MovieModel : MovieCreateModel
{
	public MovieModel(Movie movie)
		: base(movie)
	{
	}

	// Could have auto generated fields like created date, etc.
}

public class MovieCreateModel : MovieUpdateModel
{
	[JsonConstructor]
	public MovieCreateModel()
	{
	}

	public MovieCreateModel(Movie movie)
		: base(movie)
	{
		Key = movie.Key;
	}

	[Required, Length(1, 250)]
	public string? Key { get; set; }
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

	public Movie ToMovie(string key) => new Movie()
	{
		Key = key,
		Name = Name,
		Description = Description,
		Rate = (byte)(Rate * 10),
		Length = Length,
		Img = Img,
		Genres = Genres
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
	public IEnumerable<string> Genres { get; set; } = Enumerable.Empty<string>();
}
