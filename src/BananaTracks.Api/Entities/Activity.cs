namespace BananaTracks.Api.Entities;

[DynamoDBTable("BananaTracksActivities")]
internal class Activity : EntityBase
{
	[DynamoDBHashKey]
	public string UserId { get; set; } = default!;

	[DynamoDBRangeKey]
	public string ActivityId { get; set; } = Guid.NewGuid().ToString();

	public string Name { get; set; } = default!;

	public static Shared.Models.Activity ToModel(Activity activity)
	{
		return new(activity.UserId, activity.ActivityId, activity.Name);
	}
}
