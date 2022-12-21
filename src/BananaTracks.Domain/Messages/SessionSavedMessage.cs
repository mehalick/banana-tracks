namespace BananaTracks.Domain.Messages;

public class SessionSavedMessage
{
	public string UserId { get; set; } = default!;
	public string RoutineId { get; set; } = default!;
}