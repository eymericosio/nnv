using GraphQL.Types;
using Movies.Contracts;

namespace Movies.Server.Gql;

public class GqlTestType : ObjectGraphType<SampleDataModel>
{
	public GqlTestType()
	{
		Name = "Sample";
		Description = "A sample data graphtype.";

		Field(x => x.Id, nullable: true).Description("Unique key.");
		Field(x => x.Name, nullable: true).Description("Name.");
	}
}
