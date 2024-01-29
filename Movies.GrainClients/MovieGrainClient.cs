using Movies.Contracts;

namespace Movies.GrainClients;

internal class MovieGrainClient : IMovieGrainClient
{
	private readonly IClusterClient client;

	public MovieGrainClient(IClusterClient client)
	{
		this.client = client;
	}

	public Task<HashSet<Movie>> List(string? text, IEnumerable<string>? genres)
	{
		var grain = client.GetGrain<IMovieIndexGrain>(0);
		return grain.List(text, genres?.ToHashSet());
	}

	public Task<HashSet<Movie>> TopRated()
	{
		var grain = client.GetGrain<ITopMoviesGrain>(DateTimeOffset.UtcNow.ToString("yyyy-MM-dd-HH-mm"));
		return grain.List();
	}

	public Task<Movie?> Fetch(string key)
	{
		var grain = client.GetGrain<IMovieGrain>(key);
		return grain.Get();
	}

	public Task<Movie> Upsert(Movie movie)
	{
		var grain = client.GetGrain<IMovieGrain>(movie.Key);
		return grain.Set(movie);
	}

	public Task<Movie?> Delete(string key)
	{
		var grain = client.GetGrain<IMovieGrain>(key);
		return grain.Delete();
	}
}