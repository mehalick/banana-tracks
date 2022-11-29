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
			Name = routine.Name,
			Activities = routine.Activities
				.OrderBy(i => i.SortOrder)
				.Select(RoutineActivity.ToModel)
				.ToList()
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
					ActivityId = activities[i.ActivityId].ActivityId,
					Name = activities[i.ActivityId].Name,
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

	public string Name { get; set; } = default!;

	public int DurationInSeconds { get; set; }

	public int BreakInSeconds { get; set; }

	public int SortOrder { get; set; }

	public static RoutineActivityModel ToModel(RoutineActivity activity)
	{
		return new()
		{
			ActivityId = activity.ActivityId,
			Name = activity.Name,
			DurationInSeconds = activity.DurationInSeconds,
			BreakInSeconds = activity.BreakInSeconds
		};
	}

	public static RoutineActivity FromModel(RoutineActivityModel model)
	{
		return new()
		{
			ActivityId = model.ActivityId,
			Name = model.Name,
			DurationInSeconds = model.DurationInSeconds,
			BreakInSeconds = model.BreakInSeconds
		};
	}
}
