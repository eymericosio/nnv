using Microsoft.Extensions.DependencyInjection;
using Movies.Contracts;

namespace Movies.GrainClients;

public static class GrainClientsServiceCollectionExtensions
{
	public static void AddGrainClients(this IServiceCollection services)
	{
		services.AddSingleton<IMovieGrainClient, MovieGrainClient>();
	}
}
