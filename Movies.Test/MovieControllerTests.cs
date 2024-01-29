
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Controllers;
using Movies.Api.Models;
using Movies.Contracts;
using Movies.Silo;
using Orleans;

namespace Movies.Test;

public class MovieControllerTests
{
	private MovieController controller;
	private IMovieGrainClient client;

	public MovieControllerTests()
	{
		client = Substitute.For<IMovieGrainClient>();
		controller = new MovieController(client);
	}

	[Fact]
	public async Task List()
	{
		client.List(Arg.Any<string>(), Arg.Any<IEnumerable<string>>()).Returns([new Movie() { Key = "a" }]);
		var response = await controller.List("text", ["genre"]);
		var ok = Assert.IsType<OkObjectResult>(response.Result);
		var result = Assert.IsAssignableFrom<IEnumerable<MovieModel>>(ok.Value);
		Assert.NotNull(result);
		Assert.Single(result);
		await client.Received().List("text", Arg.Is<IEnumerable<string>>(gs => gs.Count() == 1 && gs.Contains("genre")));
	}

	[Fact]
	public async Task TopRated()
	{
		client.TopRated().Returns([new Movie() { Key = "a" }]);
		var response = await controller.TopRated();
		var ok = Assert.IsType<OkObjectResult>(response.Result);
		var result = Assert.IsAssignableFrom<IEnumerable<MovieModel>>(ok.Value);
		Assert.NotNull(result);
		Assert.Single(result);
	}

	[Fact]
	public async Task CreateValidationProblem()
	{
		var model = new MovieCreateModel();
		controller.ModelState.AddModelError("key", "message");
		var response = await controller.Create(model);
		var validationProblem = Assert.IsType<ObjectResult>(response.Result);
		Assert.IsType<ValidationProblemDetails>(validationProblem.Value);
	}

	[Fact]
	public async Task CreateProblem()
	{
		var model = new MovieCreateModel()
		{
			Name = "name",
			Description = "desc",
			Img = "img",
			Length = "duration",
			Rate = 5,
			Genres = [],
		};
		client.Fetch("name").Returns(new Movie());
		var response = await controller.Create(model);
		var problem = Assert.IsType<ObjectResult>(response.Result);
		Assert.IsType<ProblemDetails>(problem.Value);
	}

	[Fact]
	public async Task CreateOk()
	{
		var model = new MovieCreateModel()
		{
			Name = "name",
			Description = "desc",
			Img = "img",
			Length = "duration",
			Rate = 5,
			Genres = [],
		};
		client.Fetch("name").Returns((Movie?)null);
		client.Upsert(Arg.Any<Movie>()).Returns(new Movie());
		var response = await controller.Create(model);
		var ok = Assert.IsType<OkObjectResult>(response.Result);
		var result = Assert.IsType<MovieModel>(ok.Value);
		Assert.NotNull(result);
	}

	[Fact]
	public async Task FetchNotFound()
	{
		client.Fetch(Arg.Any<string>()).Returns((Movie?)null);
		var response = await controller.Fetch("key");
		var problem = Assert.IsType<ObjectResult>(response.Result);
		Assert.IsType<ProblemDetails>(problem.Value);
	}

	[Fact]
	public async Task FetchOk()
	{
		client.Fetch(Arg.Any<string>()).Returns(new Movie());
		var response = await controller.Fetch("key");
		var ok = Assert.IsType<OkObjectResult>(response.Result);
		var result = Assert.IsType<MovieModel>(ok.Value);
		Assert.NotNull(result);
	}

	[Fact]
	public async Task UpdateValidationProblem()
	{
		controller.ModelState.AddModelError("key", "message");
		var response = await controller.Update("key", new());
		var validationProblem = Assert.IsType<ObjectResult>(response.Result);
		Assert.IsType<ValidationProblemDetails>(validationProblem.Value);
	}

	[Fact]
	public async Task UpdateProblem()
	{
		client.Fetch(Arg.Any<string>()).Returns((Movie?)null);
		var response = await controller.Update("key", new());
		var problem = Assert.IsType<ObjectResult>(response.Result);
		Assert.IsType<ProblemDetails>(problem.Value);
	}

	[Fact]
	public async Task UpdateOk()
	{
		client.Fetch(Arg.Any<string>()).Returns(new Movie());
		client.Upsert(Arg.Any<Movie>()).Returns(new Movie());
		var response = await controller.Update("key", new() { Name = "name" });
		var ok = Assert.IsType<OkObjectResult>(response.Result);
		var result = Assert.IsType<MovieModel>(ok.Value);
		Assert.NotNull(result);
	}

	[Fact]
	public async Task DeleteNotFound()
	{
		client.Delete(Arg.Any<string>()).Returns((Movie?)null);
		var response = await controller.Delete("key");
		var problem = Assert.IsType<ObjectResult>(response.Result);
		Assert.IsType<ProblemDetails>(problem.Value);
	}

	[Fact]
	public async Task DeleteOk()
	{
		client.Delete(Arg.Any<string>()).Returns(new Movie());
		var response = await controller.Delete("key");
		var ok = Assert.IsType<OkObjectResult>(response.Result);
		var result = Assert.IsType<MovieModel>(ok.Value);
		Assert.NotNull(result);
	}
}
