using Microsoft.AspNetCore.Mvc;
using Movies.Contracts;
using Movies.Api.Models;
using GraphQL;

namespace Movies.Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
[Authorize]
public class MovieController : ControllerBase
{
	private readonly IMovieService service;

	public MovieController(IMovieService service)
	{
		this.service = service;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<MovieModel>>> List(string? search = null, [FromQuery] IEnumerable<string>? genres = null)
	{
		var movies = await service.List(search, genres);
		return Ok(movies.Select(m => new MovieModel(m)));
	}

	[HttpGet("top-rated")]
	public async Task<ActionResult<IEnumerable<MovieModel>>> TopRated()
	{
		var movies = await service.TopRated();
		return Ok(movies.Select(m => new MovieModel(m)));
	}

	[HttpPost]
	public async Task<ActionResult<MovieModel>> Create(MovieCreateModel model)
	{
		if (!ModelState.IsValid)
			return ValidationProblem(ModelState);
		var movie = await service.Fetch(model.Key!);
		if (movie.Name is not null)
			return Problem(statusCode: StatusCodes.Status409Conflict);
		movie = await service.Upsert(model.ToMovie(model.Key!));
		return Ok(new MovieModel(movie));
	}

	[HttpGet("{key}")]
	public async Task<ActionResult<MovieModel>> Fetch(string key)
	{
		var movie = await service.Fetch(key);
		if (movie.Name is null || movie.IsDeleted)
			return Problem(statusCode: StatusCodes.Status404NotFound);
		return Ok(new MovieModel(movie));
	}

	[HttpPut("{key}")]
	public async Task<ActionResult<MovieModel>> Update(string key, MovieUpdateModel model)
	{
		var movie = await service.Fetch(key);
		if (movie.Name is null || movie.IsDeleted)
			return Problem(statusCode: StatusCodes.Status404NotFound);
		if (!ModelState.IsValid)
			return ValidationProblem(ModelState);
		movie = await service.Upsert(model.ToMovie(key));
		return Ok(new MovieModel(movie));
	}

	[HttpDelete("{key}")]
	public async Task<ActionResult<MovieModel>> Delete(string key)
	{
		var movie = await service.Fetch(key);
		if (movie.Name is null || movie.IsDeleted)
			return Problem(statusCode: StatusCodes.Status404NotFound);
		movie = await service.Delete(key);
		return Ok(new MovieModel(movie));
	}
}
