using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json.Serialization.Metadata;

namespace BananaTracks.Api.Providers;

internal class QueueProvider
{
	private readonly IConfiguration _configuration;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IAmazonSQS _sqsClient;

	public QueueProvider(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IAmazonSQS sqsClient)
	{
		_configuration = configuration;
		_httpContextAccessor = httpContextAccessor;
		_sqsClient = sqsClient;
	}

	public async Task SendActivityUpdatedMessage(Activity activity, CancellationToken cancellationToken)
	{
		await SendMessage("AWS:SQS:ActivityCreatedQueueUrl", new(activity), Serializer.Default.ActivityUpdatedMessage, cancellationToken);
	}

	public async Task SendSessionSavedMessage(Session session, CancellationToken cancellationToken)
	{
		await SendMessage("AWS:SQS:SessionSavedQueueUrl", new(session), Serializer.Default.SessionSavedMessage, cancellationToken);
	}

	private async Task SendMessage<T>(string urlSetting, T activity, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken)
	{
		var url = _configuration[urlSetting];

		var json = JsonSerializer.Serialize(activity, jsonTypeInfo);

		var request = new SendMessageRequest(url, json)
		{
			MessageGroupId = _httpContextAccessor.GetUserId(),
			MessageDeduplicationId = _httpContextAccessor.GetTraceId()
		};

		await _sqsClient.SendMessageAsync(request, cancellationToken);
	}
}
