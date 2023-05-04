namespace BananaTracks.Api.Shared.Responses;

public class ListActivitiesResponse
{
	public IEnumerable<ActivityModelOld> Activities { get; set; } = Enumerable.Empty<ActivityModelOld>();
}
