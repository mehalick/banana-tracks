using BananaTracks.Core.Entities;

namespace BananaTracks.Api.Endpoints;

internal class GetRoutineById : Endpoint<GetRoutineByIdRequest, GetRoutineByIdResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Get(ApiRoutes.GetRoutineById);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public GetRoutineById(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(GetRoutineByIdRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var activities = await GetActivities(userId, cancellationToken);
		var routine = await _dynamoDbContext.LoadAsync<Routine>(userId, request.RoutineId, cancellationToken);

		Response = new()
		{
			Routine = Routine.ToModel(routine, activities)
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
