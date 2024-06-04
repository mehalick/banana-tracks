namespace BananaTracks.Api.Endpoints;

internal class AddActivity : Endpoint<AddActivityRequest>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;
	private readonly QueueProvider _queueProvider;

	public override void Configure()
	{
		Post(ApiRoutes.AddActivity);
		SerializerContext(ApiSerializer.Default);
	}

	public AddActivity(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext, QueueProvider queueProvider)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
		_queueProvider = queueProvider;
	}

	public override async Task HandleAsync(AddActivityRequest request, CancellationToken cancellationToken)
	{
		var activity = await SaveActivity(request, cancellationToken);

		await _queueProvider.SendActivityUpdatedMessage(activity, cancellationToken);

		await SendOkAsync(cancellationToken);
	}

	private async Task<Activity> SaveActivity(AddActivityRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var activity = new Activity
		{
			UserId = userId,
			Name = request.Name
		};

		await _dynamoDbContext.SaveAsync(activity, cancellationToken);

		return activity;
	}
}
