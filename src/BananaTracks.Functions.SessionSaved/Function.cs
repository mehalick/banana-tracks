using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using BananaTracks.Core.Configuration;
using BananaTracks.Core.Entities;

namespace BananaTracks.Functions.SessionSaved;

public class Function
{
	private readonly IDynamoDBContext _dynamoDbContext;

	public Function()
	{
		_dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
	}

	private static async Task Main()
	{
		var handler = new Function().FunctionHandler;
		await LambdaBootstrapBuilder.Create(handler, new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
			.Build()
			.RunAsync();
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
		var body = JsonSerializer.Deserialize(message.Body, AppJsonSerializerContext.Default.SessionSavedMessage)!;

		context.Logger.LogInformation($"Processed activity UserId: {body.UserId} ActivityId: {body.RoutineId}");

		await _dynamoDbContext.SaveAsync(new Session
		{
			UserId = body.UserId,
			RoutineId = body.RoutineId
		});

		await Task.CompletedTask;
	}
}

[JsonSerializable(typeof(SQSEvent))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
}

