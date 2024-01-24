using GraphQL.Types;

namespace Movies.Server.Gql;

public class Note
{
	public Guid Id { get; set; }
	public string Message { get; set; }
}

public class GqlNoteType : ObjectGraphType<Note>
{
	public GqlNoteType()
	{
		Name = "Note";
		Description = "Note Type";
		Field(d => d.Id, nullable: false).Description("Note Id");
		Field(d => d.Message, nullable: true).Description("Note Message");
	}
}

public class GqlNotesQuery : ObjectGraphType
{
	public GqlNotesQuery()
	{
		Field<ListGraphType<GqlNoteType>>("notes")
			.Resolve(context => new List<Note> {
				new() { Id = Guid.NewGuid(), Message = "Hello World!" },
				new() { Id = Guid.NewGuid(), Message = "Hello World! How are you?" }
			});
	}
}

public class GqlNotesSchema : Schema
{
	public GqlNotesSchema(IServiceProvider serviceProvider)
		: base(serviceProvider)
	{
		Query = serviceProvider.GetRequiredService<GqlNotesQuery>();
	}
}
