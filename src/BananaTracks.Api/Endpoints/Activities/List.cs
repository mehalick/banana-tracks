namespace BananaTracks.Api.Endpoints.Activities;

public class List : EndpointWithoutRequest<GetActivitiesResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Get(ApiRoutes.ActivitiesList);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public List(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var items = await _dynamoDbContext
			.QueryAsync<Activity>(userId)
			.GetRemainingAsync(cancellationToken);

		Response = new(items.Select(Activity.ToModel));
	}
}
