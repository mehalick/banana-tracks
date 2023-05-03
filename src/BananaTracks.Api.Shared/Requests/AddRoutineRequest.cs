namespace BananaTracks.Api.Shared.Requests;

public class AddRoutineRequest
{
	[Required]
	public string Name { get; set; } = default!;

	public List<AddRoutineRequestActivity> Activities { get; set; } = new();
}

public class AddRoutineRequestActivity
{
	[Required]
	public string Name { get; set; } = default!;

	public int DurationInSeconds { get; set; } = 300;

	public int BreakInSeconds { get; set; } = 15;
}
