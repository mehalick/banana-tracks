namespace BananaTracks.Domain.Entities;

public abstract class EntityBase
{
	public EntityStatus Status { get; set; } = EntityStatus.Active;

	[DynamoDBProperty(typeof(DateTimeUtcConverter))]
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	[DynamoDBProperty(typeof(DateTimeUtcConverter))]
	public DateTime? UpdatedAt { get; set; }

	[DynamoDBProperty(typeof(DateTimeUtcConverter))]
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
