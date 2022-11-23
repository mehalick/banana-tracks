namespace BananaTracks.Api.Entities;

[DynamoDBTable("BananaTracksRoutines")]
internal class Routine
{
	[DynamoDBHashKey]
	public string UserId { get; set; } = default!;

	[DynamoDBRangeKey]
	public string RoutineId { get; set; } = default!;

	public string Name { get; set; } = default!;

	public static Shared.Models.Routine ToModel(Routine activity)
	{
		return new(activity.UserId, activity.RoutineId, activity.Name);
	}
}