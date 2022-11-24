namespace BananaTracks.Api.Entities;

[DynamoDBTable("BananaTracksActivities")]
internal class Activity : EntityBase
{
	[DynamoDBHashKey]
	public string UserId { get; set; } = default!;

	[DynamoDBRangeKey]
	public string ActivityId { get; set; } = Guid.NewGuid().ToString();

	public string Name { get; set; } = default!;

	public static ActivityModel ToModel(Activity activity)
	{
		return new()
		{
			UserId = activity.UserId,
			ActivityId = activity.ActivityId,
			Name = activity.Name
		};
	}
}
