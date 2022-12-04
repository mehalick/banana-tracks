using BananaTracks.Core.Entities;

namespace BananaTracks.Api.Endpoints;

public class ListRoutines : EndpointWithoutRequest<ListRoutinesResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Get(ApiRoutes.ListRoutines);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public ListRoutines(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var activities = await GetActivities(userId, cancellationToken);

		var routines = await _dynamoDbContext
			.QueryAsync<Routine>(userId)
			.GetRemainingAsync(cancellationToken);

		Response = new()
		{
			Routines = routines
				.OrderBy(i => i.Name)
				.Select(i => Routine.ToModel(i, activities))
		};
	}

	private async Task<Dictionary<string, Activity>> GetActivities(string userId, CancellationToken cancellationToken)
	{
		var activities = await _dynamoDbContext
			.QueryAsync<Activity>(userId)
			.GetRemainingAsync(cancellationToken);

		return activities.ToDictionary(i => i.ActivityId, i => i);
	}
}
