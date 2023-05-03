namespace BananaTracks.Domain.Entities;

[DynamoDBTable("BananaTracksActivities")]
public class Activity : EntityBase
{
	[DynamoDBHashKey]
	public string UserId { get; set; } = default!;

	[DynamoDBRangeKey]
	public string ActivityId { get; set; } = Guid.NewGuid().ToString();

	public string RoutineId { get; set; } = default!;
	
	public string Name { get; set; } = default!;

	public string AudioUrl { get; set; } = default!;
	
	public int DurationInSeconds { get; set; }

	public int BreakInSeconds { get; set; }

	public int SortOrder { get; set; }

	public static ActivityModel ToModel(Activity activity)
	{
		return new()
		{
			UserId = activity.UserId,
			ActivityId = activity.ActivityId,
			Name = activity.Name,
			AudioUrl = activity.AudioUrl
		};
	}
}
