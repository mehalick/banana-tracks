﻿namespace BananaTracks.Domain.Entities;

[DynamoDBTable("BananaTracksSessions")]
public class Session : EntityBase
{
	[DynamoDBHashKey]
	public string UserId { get; set; } = default!;

	[DynamoDBRangeKey]
	public string SessionId { get; set; } = Guid.NewGuid().ToString();

	public string RoutineId { get; set; } = default!;

	public string RoutineName { get; set; } = default!;
}
