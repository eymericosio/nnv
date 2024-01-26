using System.Security.Claims;

namespace Movies.Api.Infrastructure;

public class MyGraphQLUserContext : Dictionary<string, object?>
{
	public ClaimsPrincipal User { get; set; }

	public MyGraphQLUserContext(ClaimsPrincipal user)
	{
		User = user;
	}
}
