using BananaTracks.Shared.Models;

namespace BananaTracks.Shared.Responses;

public record ListRoutinesResponse
{
	public IEnumerable<RoutineModel> Routines { get; set; } = Enumerable.Empty<RoutineModel>();
}
