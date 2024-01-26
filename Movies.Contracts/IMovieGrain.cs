namespace Movies.Contracts;

public interface IMovieGrain : IGrainWithStringKey
{
	Task<Movie> Get();
	Task<Movie> Set(Movie movie);
	Task<Movie> Delete();
}
