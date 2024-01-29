namespace Movies.Contracts;
using Orleans.Concurrency;

public interface IMovieIndexGrain : IGrainWithIntegerKey
{
	Task Refresh();
	Task Add(Movie movie);
	Task Remove(string key);
	Task<HashSet<Movie>> List(string? text, HashSet<string>? genres);
}
