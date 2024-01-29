using Movies.Contracts;
using Orleans;
using Orleans.Hosting;
using Orleans.TestingHost;

namespace Movies.Test;

// Would be better to tests grains independantly but harder to do without a way to mock/stub the grains
[Collection(ClusterCollection.Name)]
public class GrainTests(ClusterFixture fixture)
{
	//	Setup cluster via fixture to avoid overhead and slowness
	// see https://learn.microsoft.com/en-us/dotnet/orleans/implementation/testing
	// However it means memory data is shared across all UTs, so "collisions" possible
	private readonly TestCluster cluster = fixture.Cluster;

	[Fact]
	public async Task AddMovie()
	{
		var key = Guid.NewGuid().ToString();
		var movieGrain = cluster.GrainFactory.GetGrain<IMovieGrain>(key);
		var indexGrain = cluster.GrainFactory.GetGrain<IMovieIndexGrain>(0);
		Assert.Null(await movieGrain.Get());
		Assert.DoesNotContain(await indexGrain.List(null, null), m => m.Key == key);
		Assert.NotNull(await movieGrain.Set(new Movie()));
		Assert.NotNull(await movieGrain.Get());
		Assert.Contains(await indexGrain.List(null, null), m => m.Key == key);
	}

	[Fact]
	public async Task DeleteMovie()
	{
		var key = Guid.NewGuid().ToString();
		var movieGrain = cluster.GrainFactory.GetGrain<IMovieGrain>(key);
		var indexGrain = cluster.GrainFactory.GetGrain<IMovieIndexGrain>(0);
		Assert.Null(await movieGrain.Delete());
		await movieGrain.Set(new Movie());
		Assert.Contains(await indexGrain.List(null, null), m => m.Key == key);
		Assert.NotNull(await movieGrain.Delete());
		Assert.Null(await movieGrain.Get());
		Assert.DoesNotContain(await indexGrain.List(null, null), m => m.Key == key);
	}

	[Fact]
	public async Task IndexRefresh()
	{
		var key = Guid.NewGuid().ToString();
		var movieGrain = cluster.GrainFactory.GetGrain<IMovieGrain>(key);
		var indexGrain = cluster.GrainFactory.GetGrain<IMovieIndexGrain>(0);
		await movieGrain.Set(new Movie());
		await indexGrain.Refresh();
		Assert.Contains(await indexGrain.List(null, null), m => m.Key == key);
	}

	[Theory]
	[InlineData("Name", null, 3)]
	[InlineData("NameA", null, 1)]
	[InlineData("meA", null, 1)]
	[InlineData(null, new string[] { "G1" }, 3)]
	[InlineData(null, new string[] { "G1", "G2" }, 2)]
	[InlineData(null, new string[] { "G1", "G2", "G3" }, 1)]
	[InlineData("Name", new string[] { "G1" }, 3)]
	[InlineData("Name", new string[] { "G1", "G2" }, 2)]
	[InlineData("Name", new string[] { "G1", "G2", "G3" }, 1)]
	[InlineData("NameA", new string[] { "G1", "G2", "G3" }, 0)]
	[InlineData("NameB", new string[] { "G1", "G2", "G3" }, 0)]
	[InlineData("NameC", new string[] { "G1", "G2", "G3" }, 1)]
	public async Task IndexSearch(string? text, string[]? genres, int result)
	{
		await cluster.GrainFactory.GetGrain<IMovieGrain>("a").Set(new Movie() { Name = "NameA", Genres = ["G1"] });
		await cluster.GrainFactory.GetGrain<IMovieGrain>("b").Set(new Movie() { Name = "NameB", Genres = ["G1", "G2"] });
		await cluster.GrainFactory.GetGrain<IMovieGrain>("c").Set(new Movie() { Name = "NameC", Genres = ["G1", "G2", "G3"] });
		var indexGrain = cluster.GrainFactory.GetGrain<IMovieIndexGrain>(0);
		Assert.Equal(result, (await indexGrain.List(text, genres?.ToHashSet())).Count);
	}

	[Fact]
	public async Task TopMovies()
	{
		await cluster.GrainFactory.GetGrain<IMovieGrain>("top-a").Set(new Movie() { Rate = 1 });
		await cluster.GrainFactory.GetGrain<IMovieGrain>("top-b").Set(new Movie() { Rate = 2 });
		await cluster.GrainFactory.GetGrain<IMovieGrain>("top-c").Set(new Movie() { Rate = 3 });
		await cluster.GrainFactory.GetGrain<IMovieGrain>("top-d").Set(new Movie() { Rate = 4 });
		await cluster.GrainFactory.GetGrain<IMovieGrain>("top-e").Set(new Movie() { Rate = 5 });
		await cluster.GrainFactory.GetGrain<IMovieGrain>("top-f").Set(new Movie() { Rate = 6 });
		await cluster.GrainFactory.GetGrain<IMovieGrain>("top-g").Set(new Movie() { Rate = 7 });
		var topMovies = cluster.GrainFactory.GetGrain<ITopMoviesGrain>(Guid.NewGuid().ToString());
		var top5 = await topMovies.List();
		Assert.Equal(5, top5.Count);
		Assert.Equal("top-g", top5.First().Key);
		Assert.Equal("top-c", top5.Last().Key);
	}
}

class SiloConfigurator : ISiloConfigurator
{
	public void Configure(ISiloBuilder siloBuilder)
	{
		siloBuilder.AddMemoryGrainStorage("memoryStorage");
	}
}

public sealed class ClusterFixture : IDisposable
{
	public TestCluster Cluster { get; } = new TestClusterBuilder()
			.AddSiloBuilderConfigurator<SiloConfigurator>()
			.Build();

	public ClusterFixture() => Cluster.Deploy();

	void IDisposable.Dispose() => Cluster.StopAllSilos();
}

[CollectionDefinition(Name)]
public sealed class ClusterCollection : ICollectionFixture<ClusterFixture>
{
	public const string Name = nameof(ClusterCollection);
}
