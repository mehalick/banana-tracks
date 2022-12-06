namespace BananaTracks.Api.Endpoints;

internal class DeleteRoutine : Endpoint<DeleteRoutineRequest>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public override void Configure()
	{
		Post(ApiRoutes.DeleteRoutine);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public DeleteRoutine(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext, IWebHostEnvironment webHostEnvironment)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
		_webHostEnvironment = webHostEnvironment;
	}

	public override async Task HandleAsync(DeleteRoutineRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();
		var routine = await _dynamoDbContext.LoadAsync<Routine>(userId, request.RoutineId, cancellationToken);

		if (routine is not null)
		{
			if (_webHostEnvironment.IsDevelopment())
			{
				await _dynamoDbContext.DeleteAsync(routine, cancellationToken);
			}
			else
			{
				routine.Archive();

				await _dynamoDbContext.SaveAsync(routine, cancellationToken);
			}
		}

		await SendOkAsync(cancellationToken);
	}
}
