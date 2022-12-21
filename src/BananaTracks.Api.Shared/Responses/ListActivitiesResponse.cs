namespace BananaTracks.Api.Shared.Responses;

public class ListActivitiesResponse
{
	public IEnumerable<ActivityModel> Activities { get; set; } = Enumerable.Empty<ActivityModel>();
}
