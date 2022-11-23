using BananaTracks.Shared.Models;

namespace BananaTracks.Shared.Responses;

public record ActivitiesListResponse(IEnumerable<Activity> Activities);
