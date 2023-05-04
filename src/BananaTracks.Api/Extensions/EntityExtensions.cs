using BananaTracks.Api.Shared.Models;

namespace BananaTracks.Api.Extensions;

public static class EntityExtensions
{
	public static RoutineModel ToModel(this Routine routine, IEnumerable<Activity> activities)
	{
		return new()
		{
			UserId = routine.UserId,
			RoutineId = routine.RoutineId,
			Name = routine.Name,
			LastRunAt = routine.LastRunAt,
			Activities = activities
				.OrderBy(i => i.SortOrder)
				.Select(i => new ActivityModel
				{
					ActivityId = i.ActivityId,
					Name = i.Name,
					AudioUrl = i.AudioUrl,
					DurationInSeconds = i.DurationInSeconds,
					BreakInSeconds = i.BreakInSeconds
				})
				.ToList()
		};
	}
	
	public static ActivityModelOld ToModel(this Activity activity)
	{
		return new()
		{
			UserId = activity.UserId,
			ActivityId = activity.ActivityId,
			Name = activity.Name,
			AudioUrl = activity.AudioUrl
		};
	}
	
	public static SessionModel ToModel(this Session session)
	{
		return new()
		{
			UserId = session.UserId,
			SessionId = session.SessionId,
			RouteId = session.RoutineId,
			RoutineName = session.RoutineName,
			CreatedAt = session.CreatedAt
		};
	}
}
