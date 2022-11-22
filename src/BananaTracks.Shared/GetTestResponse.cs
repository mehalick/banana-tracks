using BananaTracks.Shared.Models;

namespace BananaTracks.Shared;

public record GetTestResponse(IEnumerable<Activity> Activities);
