namespace BananaTracks.Domain.Entities;

[DynamoDBTable("BananaTracksRoutines")]
public class Routine : EntityBase
{
	[DynamoDBHashKey]
	public string UserId { get; set; } = default!;

	[DynamoDBRangeKey]
	public string RoutineId { get; set; } = Guid.NewGuid().ToString();

	public string Name { get; set; } = default!;

	public List<RoutineActivity> Activities { get; set; } = new();

	[DynamoDBProperty(typeof(DateTimeUtcConverter))]
	public DateTime? LastRunAt { get; set; }

	public static RoutineModel ToModel(Routine routine, Dictionary<string, Activity> activities)
	{
		var routineModel = new RoutineModel
		{
			UserId = routine.UserId,
			RoutineId = routine.RoutineId,
			Name = routine.Name,
			LastRunAt = routine.LastRunAt,
			Activities = routine.Activities
				.OrderBy(i => i.SortOrder)
				.Select(i => new RoutineActivityModel
				{
					ActivityId = i.ActivityId,
					DurationInSeconds = i.DurationInSeconds,
					BreakInSeconds = i.BreakInSeconds
				})
				.ToList()
		};

		for (var i = 0; i < routineModel.Activities.Count; i++)
		{
			var routineActivityModel = routineModel.Activities[i];

			if (activities.TryGetValue(routineActivityModel.ActivityId, out var activity))
			{
				routineActivityModel.Name = activity.Name;
				routineActivityModel.AudioUrl = activity.AudioUrl;
			}
			else
			{
				routineModel.Activities.RemoveAt(i);
			}
		}

		return routineModel;
	}
}
