namespace Movies.Contracts;

[GenerateSerializer, Immutable]
public sealed record class Movie
{
	[Id(0)] public string Key { get; set; } = default!;
	[Id(1)] public string Name { get; set; } = string.Empty;
	[Id(2)] public string Description { get; set; } = string.Empty;
	[Id(3)] public byte Rate { get; set; } = 0;
	[Id(4)] public string Length { get; set; } = string.Empty;
	[Id(5)] public string Img { get; set; } = string.Empty;
	[Id(6)] public HashSet<string> Genres { get; set; } = [];
}
