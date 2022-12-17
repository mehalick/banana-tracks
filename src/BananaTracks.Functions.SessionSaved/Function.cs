using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using System.Text.Json.Serialization;

namespace BananaTracks.Functions.SessionSaved;

public class Function
{
	//private readonly IDynamoDBContext _dynamoDbContext;

	public Function()
	{
		//_dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
	}

	private static async Task Main()
	{
		var handler = new Function().FunctionHandler;
		await LambdaBootstrapBuilder.Create(handler, new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
			.Build()
			.RunAsync();
	}

	public async Task FunctionHandler(string sqsEvent, ILambdaContext context)
	{
		//foreach (var record in sqsEvent.Records)
		//{
		//	await ProcessRecordAsync(record, context);
		//}
	}

	//private async Task ProcessRecordAsync(SQSEvent.SQSMessage message, ILambdaContext context)
	//{
	//	var body = JsonSerializer.Deserialize(message.Body, AppJsonSerializerContext.Default.SessionSavedMessage)!;

	//	context.Logger.LogInformation($"Processed activity UserId: {body.UserId} ActivityId: {body.RoutineId}");

	//	await _dynamoDbContext.SaveAsync(new Session
	//	{
	//		UserId = body.UserId,
	//		RoutineId = body.RoutineId
	//	});

	//	await Task.CompletedTask;
	//}
}

//[JsonSerializable(typeof(SQSEvent))]
[JsonSerializable(typeof(string))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
}
