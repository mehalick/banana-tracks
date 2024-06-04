namespace BananaTracks.Api.Shared;

[JsonSerializable(typeof(AddActivityRequest))]
[JsonSerializable(typeof(AddRoutineRequest))]
[JsonSerializable(typeof(AddSessionRequest))]
[JsonSerializable(typeof(CreateRoutineRequest))]
[JsonSerializable(typeof(CreateRoutineResponse))]
[JsonSerializable(typeof(DeleteActivityRequest))]
[JsonSerializable(typeof(DeleteRoutineRequest))]
[JsonSerializable(typeof(GetActivityByIdRequest))]
[JsonSerializable(typeof(GetRoutineByIdRequest))]
[JsonSerializable(typeof(GetActivityByIdResponse))]
[JsonSerializable(typeof(GetRoutineByIdResponse))]
[JsonSerializable(typeof(ListActivitiesResponse))]
[JsonSerializable(typeof(ListRoutinesResponse))]
[JsonSerializable(typeof(ListSessionsResponse))]
[JsonSerializable(typeof(UpdateActivityRequest))]
[JsonSerializable(typeof(UpdateRoutineRequest))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class ApiSerializer : JsonSerializerContext
{ }
