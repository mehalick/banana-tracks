using BananaTracks.Domain.Entities;

namespace BananaTracks.Domain.Messages;

public record SessionSavedMessage : MessageBase
{
	public string RoutineId { get; set; } = default!;

	public SessionSavedMessage()
	{
	}

	public SessionSavedMessage(Session session) : base(session.UserId, session.SessionId)
	{
		RoutineId = session.RoutineId;
	}
}
