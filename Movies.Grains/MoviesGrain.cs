using Movies.Contracts;
using Orleans.Concurrency;

namespace Movies.Grains;

public class MoviesGrain : Grain, IMoviesGrain
{
	private readonly Dictionary<string, Movie> cache = [];

	[OneWay]
	public Task OnUpdate(Movie movie)
	{
		cache[movie.Key] = movie;
		return Task.CompletedTask;
	}

	public Task<HashSet<Movie>> List(string? search, IEnumerable<string>? genres) => Task.FromResult(
		cache.Values
			.Where(m => !m.IsDeleted)
			.Where(m => string.IsNullOrEmpty(search) || m.Key?.Contains(search) == true || m.Name?.Contains(search) == true || m.Description?.Contains(search) == true)
			.Where(m => genres?.Any() != true || genres.All(g => m.Genres?.Contains(g, StringComparer.OrdinalIgnoreCase) == true))
			.ToHashSet()
	);
}
