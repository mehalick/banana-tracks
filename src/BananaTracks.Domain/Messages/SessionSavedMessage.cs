using BananaTracks.Domain.Entities;

namespace BananaTracks.Domain.Messages;

public record SessionSavedMessage
{
	public string UserId { get; set; } = default!;
	public string RoutineId { get; set; } = default!;

	public SessionSavedMessage()
	{
	}

	public SessionSavedMessage(Session session)
	{
		UserId = session.UserId;
		RoutineId = session.RoutineId;
	}
}
