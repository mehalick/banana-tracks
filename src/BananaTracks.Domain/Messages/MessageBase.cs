namespace BananaTracks.Domain.Messages;

public abstract record MessageBase
{
	public string MessageGroupId { get; set; } = default!;
}