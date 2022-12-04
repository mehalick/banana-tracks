namespace BananaTracks.Core.Entities;

public abstract class EntityBase
{
	public EntityStatus Status { get; set; } = EntityStatus.Active;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}