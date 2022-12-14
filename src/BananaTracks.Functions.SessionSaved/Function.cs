using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using System.Text.Json;
using BananaTracks.Core.Configuration;
using BananaTracks.Core.Entities;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BananaTracks.Functions.SessionSaved;

public class Function
{
	private readonly IDynamoDBContext _dynamoDbContext;

	public Function()
	{
		_dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
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
