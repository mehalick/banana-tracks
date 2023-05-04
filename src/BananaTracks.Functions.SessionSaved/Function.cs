using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using BananaTracks.Domain.Entities;
using System.Text.Json;
using BananaTracks.Domain;

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
		var body = JsonSerializer.Deserialize(message.Body, Serializer.Default.SessionSavedMessage)!;

		context.Logger.LogInformation($"Processed activity UserId: {body.UserId} ActivityId: {body.RoutineId}");

		var routine = await _dynamoDbContext.LoadAsync<Routine>(body.UserId, body.RoutineId);

		if (routine is not null)
		{
			routine.LastRunAt = DateTime.UtcNow;

			await _dynamoDbContext.SaveAsync(routine);
			
			await _dynamoDbContext.SaveAsync(new Session
			{
				UserId = routine.UserId,
				RoutineId = routine.RoutineId,
				RoutineName = routine.Name
			});
		}
		
		await Task.CompletedTask;
	}
}
