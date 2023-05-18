namespace BananaTracks.Api.Shared.Requests;

public class CreateRoutineRequest
{
	[Required]
	public string Name { get; set; } = default!;
}
