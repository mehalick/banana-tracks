namespace BananaTracks.Api.Shared.Responses;

public class GetActivityByIdResponse
{
	public ActivityModelOld Activity { get; set; } = new();
}