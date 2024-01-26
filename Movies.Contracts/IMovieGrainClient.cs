namespace Movies.Contracts;

public interface IMovieService
{
	Task<HashSet<Movie>> List(string? search, IEnumerable<string>? genres);
	Task<HashSet<Movie>> TopRated();
	Task<Movie> Fetch(string key);
	Task<Movie> Upsert(Movie movie);
	Task<Movie> Delete(string key);
}
