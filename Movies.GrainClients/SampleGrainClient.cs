using Movies.Contracts;

namespace Movies.GrainClients;

public class SampleGrainClient : ISampleGrainClient
{
	private readonly IClusterClient client;

	public SampleGrainClient(IClusterClient client)
	{
		this.client = client;
	}

	public Task<SampleDataModel> Get(string id)
	{
		var grain = client.GetGrain<ISampleGrain>(id);
		return grain.Get();
	}

	public Task Set(string key, string name)
	{
		var grain = client.GetGrain<ISampleGrain>(key);
		return grain.Set(name);
	}
}