namespace BananaTracks.Api.Shared.Requests;

public class AddActivityRequest
{
	[Required]
	public string Name { get; set; } = default!;
}
