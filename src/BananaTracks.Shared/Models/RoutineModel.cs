namespace BananaTracks.Shared.Models;

public class RoutineModel
{
	public string UserId { get; set; } = default!;
	public string RoutineId { get; set; } = default!;
	public string Name { get; set; } = default!;
	public List<RoutineActivityModel> Activities { get; set; } = new();
}

public class RoutineActivityModel
{
	public ActivityModel Activity { get; set; } = default!;
	public int DurationInSeconds { get; set; }
	public int BreakInSeconds { get; set; }
}
