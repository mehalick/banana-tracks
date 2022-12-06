using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Polly;
using BananaTracks.Core.Entities;
using BananaTracks.Core.Messages;
using System.Text.Json;
using BananaTracks.Core.Configuration;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BananaTracks.Functions.ActivityCreated;

public class Function
{
	private readonly IDynamoDBContext _dynamoDbContext;
	private readonly IAmazonPolly _pollyClient;

	public Function()
	{
		_dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
		_pollyClient = new AmazonPollyClient(RegionEndpoint.USEast1);
	}

	public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
	{
		foreach (var record in sqsEvent.Records)
		{
			await ProcessRecordAsync(record, context);
		}
	}

	private async Task ProcessRecordAsync(SQSEvent.SQSMessage message, ILambdaContext context)
	{
		var body = JsonSerializer.Deserialize(message.Body, AppJsonSerializerContext.Default.ActivityCreatedMessage)!;

		context.Logger.LogInformation($"Processed activity UserId: {body.UserId} ActivityId: {body.ActivityId}");

		var activity = await _dynamoDbContext.LoadAsync<Activity>(body.UserId, body.ActivityId);

		if (activity is not null)
		{
			activity.AudioUrl = await SynthesizeSpeech(activity);

			await _dynamoDbContext.SaveAsync(activity);
		}

		await Task.CompletedTask;
	}

	private async Task<string> SynthesizeSpeech(Activity activity)
	{
		var response = await _pollyClient.StartSpeechSynthesisTaskAsync(new()
		{
			OutputFormat = OutputFormat.Mp3,
			VoiceId = VoiceId.Joanna,
			Text = activity.Name,
			OutputS3BucketName = "cdn.bananatracks.com",
			OutputS3KeyPrefix = "polly/"
		});

		// https://s3.us-east-1.amazonaws.com/cdn.bananatracks.com/polly/.some-guid.mp3

		return response.SynthesisTask.OutputUri.Replace("https://s3.us-east-1.amazonaws.com/", "https://");
	}
}
