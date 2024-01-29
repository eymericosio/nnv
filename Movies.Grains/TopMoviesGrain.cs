using Movies.Contracts;

namespace Movies.Grains;

public class TopMoviesGrain : Grain, ITopMoviesGrain
{
	private readonly IGrainFactory grainFactory;

	public TopMoviesGrain(IGrainFactory grainFactory)
	{
		this.grainFactory = grainFactory;
	}

	private readonly Dictionary<string, Movie> cache = [];

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		var movieIndex = grainFactory.GetGrain<IMovieIndexGrain>(0);
		var movies = await movieIndex.List(null, null);
		foreach (var movie in movies.OrderByDescending(m => m.Rate).Take(5))
			cache.Add(movie.Key, movie);

		await base.OnActivateAsync(cancellationToken);
	}

	public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
	{
		cache.Clear();
		return base.OnDeactivateAsync(reason, cancellationToken);
	}

	public Task<HashSet<Movie>> List() => Task.FromResult(cache.Values.ToHashSet());
}
