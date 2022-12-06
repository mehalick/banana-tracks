namespace BananaTracks.Api.Endpoints;

internal class AddRoutine : Endpoint<AddRoutineRequest>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Post(ApiRoutes.AddRoutine);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public AddRoutine(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(AddRoutineRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var routine = new Routine
		{
			UserId = userId,
			Name = request.Name,
			Activities = request.Activities
				.Select(RoutineActivity.FromModel)
				.ToList()
		};

		for (var i = 1; i <= routine.Activities.Count; i++)
		{
			routine.Activities[i].SortOrder = i;
		}

		await _dynamoDbContext.SaveAsync(routine, cancellationToken);

		await SendOkAsync(cancellationToken);
	}
}
