using BananaTracks.Shared.Models;

namespace BananaTracks.Shared.Responses;

public record RoutinesListResponse(IEnumerable<Routine> Routines);
