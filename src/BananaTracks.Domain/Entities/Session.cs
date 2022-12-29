namespace BananaTracks.Domain.Entities;

[DynamoDBTable("BananaTracksSessions")]
public class Session : EntityBase
{
	[DynamoDBHashKey]
	public string UserId { get; set; } = default!;

	[DynamoDBRangeKey]
	public string SessionId { get; set; } = Guid.NewGuid().ToString();

	public string RoutineId { get; set; } = default!;

	public string RoutineName { get; set; } = default!;

	public static SessionModel ToModel(Session session)
	{
		return new()
		{
			UserId = session.UserId,
			SessionId = session.SessionId,
			RouteId = session.RoutineId,
			RoutineName = session.RoutineName,
			CreatedAt = session.CreatedAt
		};
	}
}
