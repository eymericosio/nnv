using Movies.Contracts;
using Orleans.Runtime;
using System.Reflection;
using System.Text.Json;

namespace Movies.Silo;

public sealed class SeedStorageTask : IStartupTask
{
	private readonly IGrainFactory grainFactory;

	public SeedStorageTask(IGrainFactory grainFactory)
	{
		this.grainFactory = grainFactory;
	}

	public async Task Execute(CancellationToken cancellationToken)
	{
		var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Path should not be null");
		var moviesFileName = Path.Combine(path, "movies.json");
		var jsonData = await File.ReadAllTextAsync(moviesFileName);
		var data = JsonSerializer.Deserialize<JsonData>(jsonData, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? throw new NullReferenceException("Error parsing movies json data");
		foreach (var movie in data.Movies)
		{
			var movieGrain = grainFactory.GetGrain<IMovieGrain>(movie.key);
			await movieGrain.Set(new Movie()
			{
				Key = movie.key,
				Name = movie.name ?? string.Empty,
				Description = movie.description ?? string.Empty,
				Rate = (byte)(decimal.TryParse(movie.rate, out var rate) ? rate * 10 : 0),
				Length = movie.length ?? string.Empty,
				Img = movie.img ?? string.Empty,
				Genres = movie.genres ?? []
			});
		}
	}

	record class JsonData(IEnumerable<JsonMovie> Movies);
	record class JsonMovie
	(
		string key,
		string? name,
		string? description,
		string? rate,
		string? length,
		string? img,
		HashSet<string>? genres
	);
}
