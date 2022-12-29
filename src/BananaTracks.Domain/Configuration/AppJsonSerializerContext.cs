namespace BananaTracks.Domain.Configuration;

[JsonSerializable(typeof(ActivityUpdatedMessage))]
[JsonSerializable(typeof(AddActivityRequest))]
[JsonSerializable(typeof(AddRoutineRequest))]
[JsonSerializable(typeof(AddSessionRequest))]
[JsonSerializable(typeof(DeleteActivityRequest))]
[JsonSerializable(typeof(DeleteRoutineRequest))]
[JsonSerializable(typeof(GetActivityByIdRequest))]
[JsonSerializable(typeof(GetRoutineByIdRequest))]
[JsonSerializable(typeof(GetActivityByIdResponse))]
[JsonSerializable(typeof(GetRoutineByIdResponse))]
[JsonSerializable(typeof(ListActivitiesResponse))]
[JsonSerializable(typeof(ListRoutinesResponse))]
[JsonSerializable(typeof(ListSessionsResponse))]
[JsonSerializable(typeof(SessionSavedMessage))]
[JsonSerializable(typeof(UpdateActivityRequest))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class AppJsonSerializerContext : JsonSerializerContext
{ }
