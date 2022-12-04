using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;

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

	private async Task ProcessRecordAsync(SNSEvent.SNSRecord record, ILambdaContext context)
	{
		context.Logger.LogInformation($"Processed record {record.Sns.Message}");

		// TODO: Do interesting work based on the new message
		await Task.CompletedTask;
	}
}