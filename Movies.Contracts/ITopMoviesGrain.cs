namespace Movies.Contracts;

public interface ITopMoviesGrain : IGrainWithStringKey
{
	Task<HashSet<Movie>> TopRated();
}
