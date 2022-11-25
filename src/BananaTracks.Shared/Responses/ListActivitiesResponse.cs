using BananaTracks.Shared.Models;

namespace BananaTracks.Shared.Responses;

public class ListActivitiesResponse
{
	public IEnumerable<ActivityModel> Activities { get; set; } = Enumerable.Empty<ActivityModel>();
}
