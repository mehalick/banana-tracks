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

	public static RoutineModel ToModel(Routine routine, IEnumerable<Activity> activities)
	{
		return new()
		{
			UserId = routine.UserId,
			RoutineId = routine.RoutineId,
			Name = routine.Name,
			LastRunAt = routine.LastRunAt,
			Activities = activities
				.OrderBy(i => i.SortOrder)
				.Select(i => new RoutineActivityModel
				{
					ActivityId = i.ActivityId,
					Name = i.Name,
					AudioUrl = i.AudioUrl,
					DurationInSeconds = i.DurationInSeconds,
					BreakInSeconds = i.BreakInSeconds
				})
				.ToList()
		};
	}
}
