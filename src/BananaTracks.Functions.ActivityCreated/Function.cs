using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Polly;
using BananaTracks.Core.Entities;
using BananaTracks.Core.Messages;
using System.Text.Json;

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
		var body = JsonSerializer.Deserialize<ActivityCreatedMessage>(message.Body)!;

		context.Logger.LogInformation($"Processed activity created UserId: {body.UserId} ActivityId: {body.ActivityId}");

		var activity = await _dynamoDbContext.LoadAsync<Activity>(body.UserId, body.ActivityId);

		if (activity is not null)
		{
			var response = await _pollyClient.StartSpeechSynthesisTaskAsync(new()
			{
				OutputFormat = OutputFormat.Mp3,
				VoiceId = VoiceId.Joanna,
				Text = activity.Name,
				OutputS3BucketName = "cdn.bananatracks.com",
				OutputS3KeyPrefix = "polly/"
			});

			context.Logger.LogInformation($"Synthesized text to {response.SynthesisTask.OutputUri}");

			var url = response.SynthesisTask.OutputUri.Replace("https://s3.us-east-1.amazonaws.com/", "https://");

			activity.AudioUrl = url;

			await _dynamoDbContext.SaveAsync(activity);
		}

		await Task.CompletedTask;
	}
}
