using System.ComponentModel.DataAnnotations;

namespace BananaTracks.Shared.Requests;

public class AddActivityRequest
{
	[Required]
	public string Name { get; set; } = default!;
}