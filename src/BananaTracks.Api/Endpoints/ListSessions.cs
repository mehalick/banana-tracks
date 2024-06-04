namespace BananaTracks.Api.Endpoints;

public class ListSessions : EndpointWithoutRequest<ListSessionsResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Get(ApiRoutes.ListSessions);
		SerializerContext(ApiSerializer.Default);
	}

	public ListSessions(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var sessions = await _dynamoDbContext
			.QueryAsync<Session>(userId)
			.GetRemainingAsync(cancellationToken);

		Response = new()
		{
			Sessions = sessions
				.Active()
				.OrderByDescending(i => i.CreatedAt)
				.Select(i => i.ToModel())
		};
	}
}
