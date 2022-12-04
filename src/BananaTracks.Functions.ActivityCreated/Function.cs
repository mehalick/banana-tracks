using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using BananaTracks.Core.Entities;
using BananaTracks.Core.Messages;
using System.Text.Json;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BananaTracks.Functions.ActivityCreated;

public class Function
{
	private readonly IDynamoDBContext _dynamoDbContext;

	public Function()
	{
		_dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
	}

	public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
	{
		foreach (var record in evnt.Records)
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
			var client = new AmazonPollyClient(RegionEndpoint.USEast1);

			//var synthesizeSpeechRequest = new SynthesizeSpeechRequest
			//{
			//	OutputFormat = OutputFormat.Mp3,
			//	VoiceId = VoiceId.Joanna,
			//	Text = activity.Name
			//};

			//var synthesizeSpeechResponse = await client.SynthesizeSpeechAsync(synthesizeSpeechRequest);

			var response = await client.StartSpeechSynthesisTaskAsync(new()
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

	private static void WriteSpeechToStream(Stream audioStream, string outputFileName)
	{
		var outputStream = new FileStream(
			outputFileName,
			FileMode.Create,
			FileAccess.Write);
		byte[] buffer = new byte[2 * 1024];
		int readBytes;

		while ((readBytes = audioStream.Read(buffer, 0, 2 * 1024)) > 0)
		{
			outputStream.Write(buffer, 0, readBytes);
		}

		outputStream.Flush();
	}
}
