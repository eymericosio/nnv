namespace Movies.Contracts;
using Orleans.Concurrency;

public interface IMoviesGrain : IGrainWithIntegerKey
{
	[OneWay]
	Task OnUpdate(Movie movie);
	Task<HashSet<Movie>> List(string? search, IEnumerable<string>? genres);
}
