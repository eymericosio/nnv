using Microsoft.AspNetCore.Mvc;
using Movies.Contracts;
using Movies.Api.Models;
using GraphQL;
using System.Text.RegularExpressions;

namespace Movies.Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
[Authorize]
public class MovieController : ControllerBase
{
	private readonly IMovieGrainClient client;

	public MovieController(IMovieGrainClient client)
	{
		this.client = client;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<MovieModel>>> List(string? text = null, [FromQuery] IEnumerable<string>? genres = null)
	{
		var movies = await client.List(text, genres);
		return Ok(movies.Select(m => new MovieModel(m)));
	}

	[HttpGet("top-rated")]
	public async Task<ActionResult<IEnumerable<MovieModel>>> TopRated()
	{
		var movies = await client.TopRated();
		return Ok(movies.Select(m => new MovieModel(m)));
	}

	[HttpPost]
	public async Task<ActionResult<MovieModel>> Create(MovieCreateModel model)
	{
		if (!ModelState.IsValid)
			return ValidationProblem(ModelState);
		var key = model.GenerateKey();
		if (key is null)
			return ValidationProblem(ModelState);
		var movie = await client.Fetch(key);
		if (movie is not null)
			return Problem(statusCode: StatusCodes.Status409Conflict);
		movie = await client.Upsert(model.ToMovie(key));
		return Ok(new MovieModel(movie));
	}

	[HttpGet("{key}")]
	public async Task<ActionResult<MovieModel>> Fetch(string key)
	{
		var movie = await client.Fetch(key);
		if (movie is null)
			return Problem(statusCode: StatusCodes.Status404NotFound);
		return Ok(new MovieModel(movie));
	}

	[HttpPut("{key}")]
	public async Task<ActionResult<MovieModel>> Update(string key, MovieUpdateModel model)
	{
		if (!ModelState.IsValid)
			return ValidationProblem(ModelState);
		var movie = await client.Fetch(key);
		if (movie is null)
			return Problem(statusCode: StatusCodes.Status404NotFound);
		movie = await client.Upsert(model.ToMovie(key));
		return Ok(new MovieModel(movie));
	}

	[HttpDelete("{key}")]
	public async Task<ActionResult<MovieModel>> Delete(string key)
	{
		var movie = await client.Delete(key);
		if (movie is null)
			return Problem(statusCode: StatusCodes.Status404NotFound);
		return Ok(new MovieModel(movie));
	}
}
