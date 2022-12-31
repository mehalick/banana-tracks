namespace BananaTracks.Api.Endpoints;

internal class UpdateActivity : Endpoint<UpdateActivityRequest>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;
	private readonly QueueProvider _queueProvider;

	public override void Configure()
	{
		Post(ApiRoutes.UpdateActivity);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public UpdateActivity(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext,
		QueueProvider queueProvider)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
		_queueProvider = queueProvider;
	}

	public override async Task HandleAsync(UpdateActivityRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var activity = await _dynamoDbContext.LoadAsync<Activity>(userId, request.ActivityId, cancellationToken);

		if (activity is null)
		{
			return;
		}

		if (activity.Name != request.Name)
		{
			activity.Name = request.Name;
			activity.UpdatedAt = DateTime.UtcNow;

			await _dynamoDbContext.SaveAsync(activity, cancellationToken);

			await _queueProvider.SendActivityUpdatedMessage(activity, cancellationToken);
		}
	}
}
