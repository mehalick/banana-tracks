namespace BananaTracks.Domain.Messages;

public class ActivityCreatedMessage
{
	public string UserId { get; set; } = default!;
	public string ActivityId { get; set; } = default!;
}