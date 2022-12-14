using System.ComponentModel.DataAnnotations;

namespace BananaTracks.Shared.Requests;

public class AddSessionRequest
{
	[Required]
	public string RoutineId { get; set; } = default!;
}