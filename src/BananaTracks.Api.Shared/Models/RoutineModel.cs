namespace BananaTracks.Api.Shared.Models;

public class RoutineModel
{
	public string UserId { get; set; } = default!;
	public string RoutineId { get; set; } = default!;
	public string Name { get; set; } = default!;
	public List<RoutineActivityModel> Activities { get; set; } = new();
	public DateTime? LastRunAt { get; set; }
	public bool IsSelected { get; set; }

	[JsonIgnore]
	public string ActivitiesList => string.Join(", ", Activities.Select(i => i.Name));
}

public class RoutineActivityModel
{
	public string ActivityId { get; set; } = default!;
	public string Name { get; set; } = default!;
	public int DurationInSeconds { get; set; } = 300;
	public int BreakInSeconds { get; set; } = 30;
	public string AudioUrl { get; set; } = default!;
}
