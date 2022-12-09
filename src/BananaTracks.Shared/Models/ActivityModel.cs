namespace BananaTracks.Shared.Models;

public record ActivityModel
{
	public string UserId { get; set; } = default!;
	public string ActivityId { get; set; } = default!;
	public string Name { get; set; } = default!;
	public string AudioUrl { get; set; } = default!;
	public bool IsSelected { get; set; }
}
