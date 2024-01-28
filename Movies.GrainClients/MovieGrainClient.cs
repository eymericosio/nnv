using Movies.Contracts;

namespace Movies.GrainClients;

internal class MovieGrainClient : IMovieGrainClient
{
	private readonly IClusterClient client;

	public MovieGrainClient(IClusterClient client)
	{
		this.client = client;
	}

	public Task<HashSet<Movie>> List(string? search, IEnumerable<string>? genres)
	{
		var grain = client.GetGrain<IMoviesGrain>(0);
		return grain.List(search, genres);
	}

	public Task<HashSet<Movie>> TopRated()
	{
		var grain = client.GetGrain<ITopMoviesGrain>(DateTimeOffset.UtcNow.ToString("yyyy-MM-dd-HH-mm"));
		return grain.TopRated();
	}

	public Task<Movie> Fetch(string key)
	{
		var grain = client.GetGrain<IMovieGrain>(key);
		return grain.Get();
	}

	public Task<Movie> Upsert(Movie movie)
	{
		var grain = client.GetGrain<IMovieGrain>(movie.Key);
		return grain.Set(movie);
	}

	public Task<Movie> Delete(string key)
	{
		var grain = client.GetGrain<IMovieGrain>(key);
		return grain.Delete();
	}
}