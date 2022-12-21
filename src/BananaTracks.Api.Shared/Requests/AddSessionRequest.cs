namespace BananaTracks.Api.Shared.Requests;

public class AddSessionRequest
{
	[Required]
	public string RoutineId { get; set; } = default!;
}
