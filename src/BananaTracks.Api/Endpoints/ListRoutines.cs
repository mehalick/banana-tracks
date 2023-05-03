using BananaTracks.Domain.Extensions;

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
				.Active()
				.OrderBy(i => i.Name)
				.Select(i => Routine.ToModel(i, activities.Where(j => j.RoutineId == i.RoutineId)))
		};
	}

	private async Task<List<Activity>> GetActivities(string userId, CancellationToken cancellationToken)
	{
		var activities = await _dynamoDbContext
			.QueryAsync<Activity>(userId)
			.GetRemainingAsync(cancellationToken);

		return activities
			.Active()
			.OrderBy(i => i.SortOrder)
			.ToList();
	}
}
