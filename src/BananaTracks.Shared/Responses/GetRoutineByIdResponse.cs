using BananaTracks.Shared.Models;

namespace BananaTracks.Shared.Responses;

public class GetRoutineByIdResponse
{
	public RoutineModel Routine { get; set; } = new();
}
