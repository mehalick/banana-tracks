using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Polly;
using BananaTracks.Domain;
using BananaTracks.Domain.Entities;
using Humanizer;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BananaTracks.Functions.ActivityUpdated;

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
		var body = JsonSerializer.Deserialize(message.Body, Serializer.Default.ActivityUpdatedMessage)!;

		context.Logger.LogInformation($"Processed activity UserId: {body.UserId} ActivityId: {body.ActivityId}");

		var activity = await _dynamoDbContext.LoadAsync<Activity>(body.UserId, body.ActivityId);

		if (activity is not null)
		{
			activity.AudioUrl = await SynthesizeSpeech(activity);
			activity.UpdatedAt = DateTime.UtcNow;

			await _dynamoDbContext.SaveAsync(activity);
		}

		await Task.CompletedTask;
	}

	private async Task<string> SynthesizeSpeech(Activity activity)
	{
		var duration = TimeSpan.FromSeconds(activity.DurationInSeconds).Humanize(precision: 2);
		
		var response = await _pollyClient.StartSpeechSynthesisTaskAsync(new()
		{
			OutputFormat = OutputFormat.Mp3,
			VoiceId = VoiceId.Joanna,
			Text = $"Start {activity.Name}, for {duration}",
			OutputS3BucketName = "cdn.bananatracks.com",
			OutputS3KeyPrefix = $"polly/{activity.ActivityId}" 
		});

		// https://s3.us-east-1.amazonaws.com/cdn.bananatracks.com/polly/.some-guid.mp3

		return response.SynthesisTask.OutputUri.Replace("https://s3.us-east-1.amazonaws.com/", "https://");
	}
}
