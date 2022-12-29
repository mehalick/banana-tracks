namespace BananaTracks.Api.Shared.Models;

public record SessionModel
{
	public string UserId { get; set; } = default!;
	public string SessionId { get; set; } = default!;
	public string RouteId { get; set; } = default!;
	public string RoutineName { get; set; } = default!;
	public DateTime CreatedAt { get; set; }
}
