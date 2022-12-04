using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using BananaTracks.Core.Topics;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BananaTracks.Functions.ActivityCreated;

public class Function
{
	public Function()
	{

	}

	public async Task FunctionHandler(SNSEvent evnt, ILambdaContext context)
	{
		foreach (var record in evnt.Records)
		{
			await ProcessRecordAsync(record, context);
		}
	}

	private static async Task ProcessRecordAsync(SNSEvent.SNSRecord record, ILambdaContext context)
	{
		var message = JsonSerializer.Deserialize<ActivityCreatedMessage>(record.Sns.Message)!;

		context.Logger.LogInformation($"Processed activity created UserId: {message.UserId} ActivityId: {message.ActivityId}");

		// TODO: Do interesting work based on the new message
		await Task.CompletedTask;
	}
}