using BananaTracks.Shared.Models;

namespace BananaTracks.Core.Entities;

public class RoutineActivity
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
