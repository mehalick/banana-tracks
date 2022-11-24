namespace BananaTracks.Api.Endpoints.Activities;

internal class Add : Endpoint<ActivityAddRequest>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Post(ApiRoutes.ActivitiesAdd);
		//SerializerContext(AppJsonSerializerContext.Default);
	}

	public Add(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(ActivityAddRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var activity = new Activity
		{
			UserId = userId!,
			Name = request.Name
		};

		await _dynamoDbContext.SaveAsync(activity, cancellationToken);

		await SendOkAsync(cancellationToken);
	}
}