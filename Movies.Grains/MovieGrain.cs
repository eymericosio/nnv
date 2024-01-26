using Movies.Contracts;
using Orleans.Runtime;

namespace Movies.Grains;

public class MovieGrain : Grain, IMovieGrain
{
	private readonly IGrainFactory grainFactory;
	private readonly IPersistentState<Movie> store;

	public MovieGrain
	(
		IGrainFactory grainFactory,
		[PersistentState("movie", "memoryStorage")] IPersistentState<Movie> store
	)
	{
		this.grainFactory = grainFactory;
		this.store = store;
	}

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		store.State = store.State with { Key = this.GetPrimaryKeyString() };
		await base.OnActivateAsync(cancellationToken);
	}

	public Task<Movie> Get() => Task.FromResult(store.State);

	public async Task<Movie> Set(Movie movie)
	{
		store.State = movie with { Key = store.State.Key };
		await store.WriteStateAsync();

		var moviesGrain = grainFactory.GetGrain<IMoviesGrain>(0);
		await moviesGrain.OnUpdate(store.State);

		return store.State;
	}

	// clearing State would not necessarily delete data in storage
	// probably better to rely on storage provider cron job or an external job/trigger to actually cleanup
	// also possible to keep data (for legal reasons for example)
	public async Task<Movie> Delete() => await Set(store.State with { IsDeleted = true });
}
