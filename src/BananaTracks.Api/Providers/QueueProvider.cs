using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json.Serialization.Metadata;
using BananaTracks.Domain.Messages;

namespace BananaTracks.Api.Providers;

internal class QueueProvider
{
	private readonly IConfiguration _configuration;
	private readonly IAmazonSQS _sqsClient;
	private readonly ILogger<QueueProvider> _log;

	public QueueProvider(IConfiguration configuration, IAmazonSQS sqsClient, ILogger<QueueProvider> log)
	{
		_configuration = configuration;
		_sqsClient = sqsClient;
		_log = log;
	}

	public async Task SendActivityUpdatedMessage(Activity activity, CancellationToken cancellationToken)
	{
		await SendMessage("AWS:SQS:ActivityCreatedQueueUrl", new(activity), Serializer.Default.ActivityUpdatedMessage, cancellationToken);
	}

	public async Task SendSessionSavedMessage(Session session, CancellationToken cancellationToken)
	{
		await SendMessage("AWS:SQS:SessionSavedQueueUrl", new(session), Serializer.Default.SessionSavedMessage, cancellationToken);
	}

	private async Task<SendMessageResponse> SendMessage<T>(string urlSetting, T message, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken)
		where T : MessageBase
	{
		var url = _configuration[urlSetting];

		var json = JsonSerializer.Serialize(message, jsonTypeInfo);

		var request = new SendMessageRequest(url, json)
		{
			MessageGroupId = message.GroupId,
			MessageDeduplicationId = message.DeduplicationId
		};

		var response = await _sqsClient.SendMessageAsync(request, cancellationToken);
		
		_log.LogInformation("SQS {@Message} sent", new
		{
			QueueUrl = url,
			message.DeduplicationId,
			response.MessageId,
			response.HttpStatusCode
		});
		
		return response;
	}
}
