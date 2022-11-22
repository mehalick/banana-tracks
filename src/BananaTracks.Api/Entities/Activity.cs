using Amazon.DynamoDBv2.DataModel;

namespace BananaTracks.Api.Entities;

[DynamoDBTable("BananaTracksActivities")]
internal class Activity
{
	[DynamoDBHashKey]
	public string UserId { get; set; } = default!;

	[DynamoDBRangeKey]
	public string ActivityId { get; set; } = default!;

	public string Name { get; set; } = default!;

	public static Shared.Models.Activity Create(Activity activity)
	{
		return new(activity.UserId, activity.ActivityId, activity.Name);
	}
}