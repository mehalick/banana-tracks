namespace BananaTracks.Api.Shared.Responses;

public class ListRoutinesResponse
{
	public IEnumerable<RoutineModel> Routines { get; set; } = Enumerable.Empty<RoutineModel>();
}
