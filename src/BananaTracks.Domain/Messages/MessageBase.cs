namespace BananaTracks.Domain.Messages;

public abstract record MessageBase
{
	public string UserId { get; set; } = default!;
	public string GroupId { get; set; } = default!;
	public string DeduplicationId { get; set; } = default!;

	protected MessageBase()
	{
		
	}
	
	protected MessageBase(string userId, string entityId)
	{
		UserId = userId;
		GroupId = userId;
		DeduplicationId = entityId;
	}
}
