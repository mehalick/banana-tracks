using BananaTracks.Shared.Models;

namespace BananaTracks.Shared;

public record GetActivitiesResponse(IEnumerable<Activity> Activities);
