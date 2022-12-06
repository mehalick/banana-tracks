namespace BananaTracks.Api.Endpoints;

internal class DeleteActivity : Endpoint<DeleteActivityRequest>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public override void Configure()
	{
		Post(ApiRoutes.DeleteActivity);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public DeleteActivity(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext, IWebHostEnvironment webHostEnvironment)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
		_webHostEnvironment = webHostEnvironment;
	}

	public override async Task HandleAsync(DeleteActivityRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();
		var activity = await _dynamoDbContext.LoadAsync<Activity>(userId, request.ActivityId, cancellationToken);

		if (activity is not null)
		{
			if (_webHostEnvironment.IsDevelopment())
			{
				await _dynamoDbContext.DeleteAsync(activity, cancellationToken);
			}
			else
			{
				activity.Status = EntityStatus.Archived;
				await _dynamoDbContext.SaveAsync(activity, cancellationToken);
			}
		}

		await SendOkAsync(cancellationToken);
	}
}
