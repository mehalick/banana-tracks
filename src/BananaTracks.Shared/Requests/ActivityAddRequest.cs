using System.ComponentModel.DataAnnotations;

namespace BananaTracks.Shared.Requests;

public class ActivityAddRequest
{
	[Required]
	public string Name { get; set; } = default!;
}