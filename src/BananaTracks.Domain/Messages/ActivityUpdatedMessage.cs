using BananaTracks.Domain.Entities;

namespace BananaTracks.Domain.Messages;

public record ActivityUpdatedMessage
{
	public string UserId { get; set; } = default!;
	public string ActivityId { get; set; } = default!;

	private ActivityUpdatedMessage()
	{
	}

	public ActivityUpdatedMessage(Activity activity)
	{
		UserId = activity.UserId;
		ActivityId = activity.ActivityId;
	}
}