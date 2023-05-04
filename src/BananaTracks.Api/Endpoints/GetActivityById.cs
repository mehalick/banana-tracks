namespace BananaTracks.Api.Endpoints;

internal class GetActivityById : Endpoint<GetActivityByIdRequest, GetActivityByIdResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Get(ApiRoutes.GetActivityById);
		SerializerContext(Serializer.Default);
	}

	public GetActivityById(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(GetActivityByIdRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var activity = await _dynamoDbContext.LoadAsync<Activity>(userId, request.ActivityId, cancellationToken);

		Response = new()
		{
			Activity = activity.ToModel()
		};
	}
}
