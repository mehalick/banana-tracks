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
			UserId = userId!,
			Name = request.Name,
			Activities = new()
			{
				new() { ActivityId = "2cdc922a-d93c-460e-a378-c4c17d210b6c", DurationInSeconds = 300, BreakInSeconds = 30, SortOrder = 1},
				new() { ActivityId = "859604f4-6aa9-4601-8d18-da0eb83a9aa8", DurationInSeconds = 300, BreakInSeconds = 30, SortOrder = 2},
				new() { ActivityId = "5983364a-fad7-4382-bac8-c3b1ce73376c", DurationInSeconds = 300, BreakInSeconds = 0, SortOrder = 3}
			}
		};

		await _dynamoDbContext.SaveAsync(routine, cancellationToken);

		await SendOkAsync(cancellationToken);
	}
}