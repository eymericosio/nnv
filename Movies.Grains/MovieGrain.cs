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

	public Task<Movie?> Get() => Task.FromResult(store.RecordExists ? store.State : null);

	public async Task<Movie> Set(Movie movie)
	{
		store.State = movie with { Key = this.GetPrimaryKeyString() };
		await store.WriteStateAsync();

		var movieIndex = grainFactory.GetGrain<IMovieIndexGrain>(0);
		await movieIndex.Add(store.State);

		return store.State;
	}

	public async Task<Movie?> Delete()
	{
		if (!store.RecordExists)
			return null;

		var movie = store.State with { };

		var movieIndex = grainFactory.GetGrain<IMovieIndexGrain>(0);
		await movieIndex.Remove(store.State.Key);

		await store.ClearStateAsync();
		return movie;
	}
}
