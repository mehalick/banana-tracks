using Amazon.SQS;

namespace BananaTracks.Api.Endpoints;

internal class AddSession : Endpoint<AddSessionRequest>
{
	private readonly IConfiguration _configuration;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IAmazonSQS _sqsClient;

	public override void Configure()
	{
		Post(ApiRoutes.AddSession);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public AddSession(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IAmazonSQS sqsClient)
	{
		_configuration = configuration;
		_httpContextAccessor = httpContextAccessor;
		_sqsClient = sqsClient;
	}

	public override async Task HandleAsync(AddSessionRequest request, CancellationToken cancellationToken)
	{
		var session = new Session
		{
			UserId = _httpContextAccessor.GetUserId(),
			RoutineId = request.RoutineId
		};

		await SendSessionSavedMessage(session, cancellationToken);

		await SendOkAsync(cancellationToken);
	}

	private async Task SendSessionSavedMessage(Session session, CancellationToken cancellationToken)
	{
		var url = _configuration["AWS:SQS:SessionSavedQueueUrl"];

		var json = JsonSerializer.Serialize(new()
		{
			UserId = session.UserId,
			RoutineId = session.RoutineId
		}, AppJsonSerializerContext.Default.SessionSavedMessage);

		await _sqsClient.SendMessageAsync(url, json, cancellationToken);
	}
}
