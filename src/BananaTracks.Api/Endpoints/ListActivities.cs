using BananaTracks.Core.Entities;

namespace BananaTracks.Api.Endpoints;

public class ListActivities : EndpointWithoutRequest<ListActivitiesResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Get(ApiRoutes.ListActivities);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public ListActivities(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var activities = await _dynamoDbContext
			.QueryAsync<Activity>(userId)
			.GetRemainingAsync(cancellationToken);

		Response = new()
		{
			Activities = activities.OrderBy(i => i.Name).Select(Activity.ToModel)
		};
	}
}
