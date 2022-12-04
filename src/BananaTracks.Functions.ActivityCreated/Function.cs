using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.SQSEvents;
using BananaTracks.Core.Topics;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BananaTracks.Functions.ActivityCreated;

public class Function
{
	public Function()
	{

	}

	public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
	{
		foreach (var record in evnt.Records)
		{
			await ProcessRecordAsync(record, context);
		}
	}

	private static async Task ProcessRecordAsync(SQSEvent.SQSMessage message, ILambdaContext context)
	{
		var body = JsonSerializer.Deserialize<ActivityCreatedMessage>(message.Body)!;

		context.Logger.LogInformation($"Processed activity created UserId: {body.UserId} ActivityId: {body.ActivityId}");

		// TODO: Do interesting work based on the new message
		await Task.CompletedTask;
	}
}