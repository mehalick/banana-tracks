namespace BananaTracks.Api.Endpoints;

internal class CreateRoutine : Endpoint<CreateRoutineRequest, CreateRoutineResponse>
{
	public override void Configure()
	{
		Post(ApiRoutes.CreateRoutine);
		SerializerContext(Serializer.Default);
	}
	
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;
	private readonly ILogger<CreateRoutine> _log;
	
	public CreateRoutine(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext, ILogger<CreateRoutine> log)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
		_log = log;
	}
	
	public override async Task HandleAsync(CreateRoutineRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var routine = new Routine
		{
			UserId = userId,
			Name = request.Name
		};
		
		_log.LogInformation("Creating {@Routine}", routine);
		
		await _dynamoDbContext.SaveAsync(routine, cancellationToken);
		
		_log.LogInformation("Created {@Routine}", routine);

		await SendOkAsync(new()
		{
			RoutineId = routine.RoutineId
		}, cancellationToken);
	}
}
