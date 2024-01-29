using Movies.Contracts;
using Orleans.Concurrency;
using Orleans.Runtime;

namespace Movies.Grains;

public class MovieIndexGrain : Grain, IMovieIndexGrain
{
	private readonly IGrainFactory grainFactory;
	private readonly IPersistentState<HashSet<string>> store;

	public MovieIndexGrain
	(
		IGrainFactory grainFactory,
		[PersistentState("movies", "memoryStorage")] IPersistentState<HashSet<string>> store
	)
	{
		this.grainFactory = grainFactory;
		this.store = store;
	}

	private readonly Dictionary<string, Movie> cache = [];

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await Refresh();
		await base.OnActivateAsync(cancellationToken);
	}

	public async Task Refresh()
	{
		if (store.State.Count <= 0)
			return;

		cache.Clear();

		await Parallel.ForEachAsync(
			store.State,
			async (key, _) =>
			{
				var movieGrain = grainFactory.GetGrain<IMovieGrain>(key);
				var movie = await movieGrain.Get();
				if (movie is not null)
					cache[key] = movie;
			});
	}

	public async Task Add(Movie movie)
	{
		store.State.Add(movie.Key);
		cache[movie.Key] = movie;

		await store.WriteStateAsync();
	}

	public async Task Remove(string key)
	{
		store.State.Remove(key);
		cache.Remove(key);

		await store.WriteStateAsync();
	}

	public Task<HashSet<Movie>> List(string? text, HashSet<string>? genres) => Task.FromResult(
		cache.Values
			.Where(m => string.IsNullOrEmpty(text) || m.Key?.Contains(text) == true || m.Name?.Contains(text) == true || m.Description?.Contains(text) == true)
			.Where(m => genres?.Any() != true || genres.All(g => m.Genres?.Contains(g, StringComparer.OrdinalIgnoreCase) == true))
			.ToHashSet()
	);
}
