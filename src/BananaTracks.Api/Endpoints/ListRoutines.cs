namespace BananaTracks.Api.Endpoints;

public class ListRoutines : EndpointWithoutRequest<ListRoutinesResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Get(ApiRoutes.RoutinesList);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public ListRoutines(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var items = await _dynamoDbContext
			.QueryAsync<Routine>(userId)
			.GetRemainingAsync(cancellationToken);

		Response = new()
		{
			Routines = items.OrderBy(i => i.Name).Select(Routine.ToModel)
		};
	}
}
