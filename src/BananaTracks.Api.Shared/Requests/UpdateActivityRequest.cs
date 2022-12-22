namespace BananaTracks.Api.Shared.Requests;

public class UpdateActivityRequest
{
	[Required]
	public string ActivityId { get; set; }= default!;

	[Required]
	public string Name { get; set; } = default!;
}