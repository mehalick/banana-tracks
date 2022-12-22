namespace BananaTracks.Api.Endpoints;

internal class AddSession : Endpoint<AddSessionRequest>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly QueueProvider _queueProvider;

	public override void Configure()
	{
		Post(ApiRoutes.AddSession);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public AddSession(IHttpContextAccessor httpContextAccessor, QueueProvider queueProvider)
	{
		_httpContextAccessor = httpContextAccessor;
		_queueProvider = queueProvider;
	}

	public override async Task HandleAsync(AddSessionRequest request, CancellationToken cancellationToken)
	{
		var session = new Session
		{
			UserId = _httpContextAccessor.GetUserId(),
			RoutineId = request.RoutineId
		};

		await _queueProvider.SendSessionSavedMessage(session, cancellationToken);

		await SendOkAsync(cancellationToken);
	}
}
