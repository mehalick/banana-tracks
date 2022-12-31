namespace BananaTracks.Api.Endpoints;

public class UpdateRoutine : Endpoint<UpdateRoutineRequest>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Post(ApiRoutes.UpdateRoutine);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public UpdateRoutine(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(UpdateRoutineRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var routine = await _dynamoDbContext.LoadAsync<Routine>(userId, request.RoutineId, cancellationToken);

		if (routine is null)
		{
			return;
		}

		if (routine.Name != request.Name)
		{
			routine.Name = request.Name;

			await _dynamoDbContext.SaveAsync(routine, cancellationToken);
		}
	}
}
