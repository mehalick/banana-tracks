using System.Diagnostics;
using Activity = BananaTracks.Domain.Entities.Activity;

namespace BananaTracks.Api.Endpoints;

public class ListActivities : EndpointWithoutRequest<ListActivitiesResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;
	private readonly ActivitySource _activitySource;

	public override void Configure()
	{
		Get(ApiRoutes.ListActivities);
		SerializerContext(Serializer.Default);
	}

	public ListActivities(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;

		_activitySource = new("BananaTracks", "1.0");
	}

	public override async Task HandleAsync(CancellationToken cancellationToken)
	{
		using var activity = _activitySource.StartActivity(nameof(ListActivities), ActivityKind.Server);

		var userId = _httpContextAccessor.GetUserId();

		List<Activity> activities;

		using (_activitySource.StartActivity("QueryDynamoDB"))
		{
			activities = await _dynamoDbContext
				.QueryAsync<Activity>(userId)
				.GetRemainingAsync(cancellationToken);
		}

		Response = new()
		{
			Activities = activities
				.Active()
				.OrderBy(i => i.Name)
				.Select(i => i.ToModel())
		};
	}
}
