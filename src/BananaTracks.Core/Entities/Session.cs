using Amazon.DynamoDBv2.DataModel;

namespace BananaTracks.Core.Entities;

[DynamoDBTable("BananaTracksSessions")]
public class Session : EntityBase
{
	[DynamoDBHashKey]
	public string UserId { get; set; } = default!;

	[DynamoDBRangeKey]
	public string SessionId { get; set; } = Guid.NewGuid().ToString();

	public string RoutineId { get; set; } = default!;
}
