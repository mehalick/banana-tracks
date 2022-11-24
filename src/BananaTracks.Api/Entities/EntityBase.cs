namespace BananaTracks.Api.Entities;

internal abstract class EntityBase
{
	public EntityStatus Status { get; set; } = EntityStatus.Active;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}