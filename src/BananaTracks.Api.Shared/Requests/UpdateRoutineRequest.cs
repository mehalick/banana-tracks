namespace BananaTracks.Api.Shared.Requests;

public class UpdateRoutineRequest
{
	[Required]
	public string RoutineId { get; set; }= default!;

	[Required]
	public string Name { get; set; } = default!;
}
