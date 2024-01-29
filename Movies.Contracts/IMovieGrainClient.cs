namespace Movies.Contracts;

public interface IMovieGrainClient
{
	Task<HashSet<Movie>> List(string? text, IEnumerable<string>? genres);
	Task<HashSet<Movie>> TopRated();
	Task<Movie?> Fetch(string key);
	Task<Movie> Upsert(Movie movie);
	Task<Movie?> Delete(string key);
}
