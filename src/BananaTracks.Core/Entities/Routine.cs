using Amazon.DynamoDBv2.DataModel;
using BananaTracks.Shared.Models;

namespace BananaTracks.Core.Entities;

[DynamoDBTable("BananaTracksRoutines")]
public class Routine : EntityBase
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
				.ToList<RoutineActivityModel>()
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