namespace BananaTracks.Domain.Entities;

[DynamoDBTable("BananaTracksRoutines")]
public class Routine : EntityBase
{
	[DynamoDBHashKey]
	public string UserId { get; set; } = default!;

	[DynamoDBRangeKey]
	public string RoutineId { get; set; } = Guid.NewGuid().ToString();

	public string Name { get; set; } = default!;

	[DynamoDBProperty(typeof(DateTimeUtcConverter))]
	public DateTime? LastRunAt { get; set; }
}
