namespace BananaTracks.Api.Shared.Responses;

public class ListSessionsResponse
{
	public IEnumerable<SessionModel> Sessions { get; set; } = Enumerable.Empty<SessionModel>();
}
