namespace BananaTracks.Api.Shared.Requests;

public class AddRoutineRequest
{
	[Required]
	public string Name { get; set; } = default!;

	public List<RoutineActivityModel> Activities { get; set; } = new();
}
