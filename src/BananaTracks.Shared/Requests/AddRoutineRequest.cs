using System.ComponentModel.DataAnnotations;
using BananaTracks.Shared.Models;

namespace BananaTracks.Shared.Requests;

public class AddRoutineRequest
{
	[Required]
	public string Name { get; set; } = default!;

	public List<RoutineActivityModel> Activities { get; set; } = new();
}