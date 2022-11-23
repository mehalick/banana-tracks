using BananaTracks.Shared.Responses;

namespace BananaTracks.Api.Endpoints.Routines;

public class List : EndpointWithoutRequest<RoutinesListResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Get(ApiRoutes.RoutinesList);
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
			.QueryAsync<Routine>(userId)
			.GetRemainingAsync(cancellationToken);

		Response = new(items.Select(Routine.ToModel));
	}
}
