using BananaTracks.Domain.Entities;

namespace BananaTracks.Domain.Messages;

public record ActivityUpdatedMessage : MessageBase
{
	public string ActivityId { get; set; } = default!;

	public ActivityUpdatedMessage()
	{
	}

	public ActivityUpdatedMessage(Activity activity) : base(activity.UserId, activity.ActivityId)
	{
		ActivityId = activity.ActivityId;
	}
}
