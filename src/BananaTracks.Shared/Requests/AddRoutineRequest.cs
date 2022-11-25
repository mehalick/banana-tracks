using System.ComponentModel.DataAnnotations;

namespace BananaTracks.Shared.Requests;

public class AddRoutineRequest
{
	[Required]
	public string Name { get; set; } = default!;
}
