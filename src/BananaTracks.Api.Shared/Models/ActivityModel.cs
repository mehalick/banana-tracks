namespace BananaTracks.Api.Shared.Models;

public class ActivityModel
{
	public string ActivityId { get; set; } = default!;
	public string Name { get; set; } = default!;
	public int DurationInSeconds { get; set; } = 300;
	public int BreakInSeconds { get; set; } = 30;
	public string AudioUrl { get; set; } = default!;
}
