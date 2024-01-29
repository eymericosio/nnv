using Movies.Contracts;
using Movies.Silo;
using Orleans;

namespace Movies.Test;

public class SeedStorageTaskTest
{
	[Fact]
	public async Task Execute()
	{
		var grainFactory = Substitute.For<IGrainFactory>();
		var task = new SeedStorageTask(grainFactory);
		await task.Execute(CancellationToken.None);
		grainFactory.Received(3).GetGrain<IMovieGrain>(Arg.Any<string>());
	}
}
