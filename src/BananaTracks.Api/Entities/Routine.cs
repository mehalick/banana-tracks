namespace BananaTracks.Api.Entities;

[DynamoDBTable("BananaTracksRoutines")]
internal class Routine : EntityBase
{
	[DynamoDBHashKey]
	public string UserId { get; set; } = default!;

	[DynamoDBRangeKey]
	public string RoutineId { get; set; } = Guid.NewGuid().ToString();

	public string Name { get; set; } = default!;

	//public string ActivitiesJson { get; set; } = default!;

	//public List<RoutineActivity> Activities
	//{
	//	get
	//	{
	//		if (string.IsNullOrWhiteSpace(ActivitiesJson))
	//		{
	//			return new();
	//		}

	//		return JsonSerializer.Deserialize(ActivitiesJson, AppJsonSerializerContext.Default.ListRoutineActivity) ?? new List<RoutineActivity>();
	//	}
	//	set => ActivitiesJson = JsonSerializer.Serialize(value, AppJsonSerializerContext.Default.ListRoutineActivity);
	//}

	public List<RoutineActivity> Activities { get; set; } = new List<RoutineActivity>();

	public static RoutineModel ToModel(Routine routine)
	{
		return new()
		{
			UserId = routine.UserId,
			RoutineId = routine.RoutineId,
			Name = routine.Name
		};
	}

	public static RoutineModel ToModel(Routine routine, Dictionary<string, Activity> activities)
	{
		return new()
		{
			UserId = routine.UserId,
			RoutineId = routine.RoutineId,
			Name = routine.Name,
			Activities = routine.Activities
				.OrderBy(i => i.SortOrder)
				.Select(i => new RoutineActivityModel
				{
					Activity = Activity.ToModel(activities[i.ActivityId]),
					DurationInSeconds = i.DurationInSeconds,
					BreakInSeconds = i.BreakInSeconds
				})
				.ToList()
		};
	}
}

internal class RoutineActivity
{
	public string ActivityId { get; set; } = default!;

	public int DurationInSeconds { get; set; }

	public int BreakInSeconds { get; set; }

	public int SortOrder { get; set; }
}
