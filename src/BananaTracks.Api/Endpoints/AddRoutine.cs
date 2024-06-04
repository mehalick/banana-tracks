namespace BananaTracks.Api.Endpoints;

internal class AddRoutine : Endpoint<AddRoutineRequest>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;
	private readonly QueueProvider _queueProvider;
	private readonly ILogger<AddRoutine> _log;

	public override void Configure()
	{
		Post(ApiRoutes.AddRoutine);
		SerializerContext(ApiSerializer.Default);
	}

	public AddRoutine(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext, QueueProvider queueProvider, ILogger<AddRoutine> log)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
		_queueProvider = queueProvider;
		_log = log;
	}

	public override async Task HandleAsync(AddRoutineRequest request, CancellationToken cancellationToken)
	{
		_log.LogInformation("Adding routing {RoutineName}", request.Name);
		
		var userId = _httpContextAccessor.GetUserId();

		var routine = new Routine
		{
			UserId = userId,
			Name = request.Name
		};
		
		await _dynamoDbContext.SaveAsync(routine, cancellationToken);

		for (var i = 0; i < request.Activities.Count; i++)
		{
			var activity = new Activity
			{
				UserId = userId,
				RoutineId = routine.RoutineId,
				Name = request.Activities[i].Name,
				SortOrder = i + 1,
				DurationInSeconds = request.Activities[i].DurationInSeconds,
				BreakInSeconds = request.Activities[i].BreakInSeconds
			};
			
			await _dynamoDbContext.SaveAsync(activity, cancellationToken);
			await _queueProvider.SendActivityUpdatedMessage(activity, cancellationToken);
		}

		await SendOkAsync(cancellationToken);
	}
}
