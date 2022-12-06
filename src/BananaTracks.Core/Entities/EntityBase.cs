namespace BananaTracks.Core.Entities;

public abstract class EntityBase
{
	public EntityStatus Status { get; set; } = EntityStatus.Active;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; set; }
	public DateTime? ArchivedAt { get; set; }

	public void Archive()
	{
		if (Status == EntityStatus.Archived)
		{
			return;
		}

		Status = EntityStatus.Archived;
		ArchivedAt = DateTime.UtcNow;
	}
}
