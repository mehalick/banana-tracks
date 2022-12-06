using System.Text.Json.Serialization;
using BananaTracks.Core.Messages;
using BananaTracks.Shared.Requests;
using BananaTracks.Shared.Responses;

namespace BananaTracks.Core.Configuration;

[JsonSerializable(typeof(ActivityCreatedMessage))]
[JsonSerializable(typeof(AddActivityRequest))]
[JsonSerializable(typeof(AddRoutineRequest))]
[JsonSerializable(typeof(DeleteActivityRequest))]
[JsonSerializable(typeof(DeleteRoutineRequest))]
[JsonSerializable(typeof(GetRoutineByIdResponse))]
[JsonSerializable(typeof(ListActivitiesResponse))]
[JsonSerializable(typeof(ListRoutinesResponse))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class AppJsonSerializerContext : JsonSerializerContext
{ }
