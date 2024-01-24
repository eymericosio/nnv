using Microsoft.AspNetCore.Mvc;
using Movies.Contracts;

namespace Movies.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SampleDataController : ControllerBase
{
	private readonly ISampleGrainClient client;

	public SampleDataController(ISampleGrainClient client)
	{
		this.client = client;
	}

	// GET api/sampledata/1234
	[HttpGet("{id}")]
	public async Task<ActionResult<SampleDataModel>> Get(string id)
	{
		var result = await client.Get(id);
		return Ok(result);
	}

	// POST api/sampledata/1234
	[HttpPost("{id}")]
	public async Task<ActionResult> Set([FromRoute] string id, [FromForm] string name)
	{
		await client.Set(id, name);
		return Ok();
	}
}
