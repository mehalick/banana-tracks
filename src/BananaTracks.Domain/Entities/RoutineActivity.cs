namespace BananaTracks.Domain.Entities;

public class RoutineActivity
{
	public string ActivityId { get; set; } = default!;

	public int DurationInSeconds { get; set; }

	public int BreakInSeconds { get; set; }

	public int SortOrder { get; set; }

	public static RoutineActivity FromModel(RoutineActivityModel model)
	{
		return new()
		{
			ActivityId = model.ActivityId,
			DurationInSeconds = model.DurationInSeconds,
			BreakInSeconds = model.BreakInSeconds
		};
	}
}
