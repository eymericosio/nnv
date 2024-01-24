using Movies.Contracts;

namespace Movies.Grains;

public class SampleGrain : Grain<SampleDataModel>, ISampleGrain
{
	public Task<SampleDataModel> Get()
	{
		return Task.FromResult(State);
	}

	public Task Set(string name)
	{
		State = State with { Id = this.GetPrimaryKeyString(), Name = name };
		return Task.CompletedTask;
	}
}
