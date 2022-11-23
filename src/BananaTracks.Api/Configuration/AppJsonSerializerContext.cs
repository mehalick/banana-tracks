using System.Text.Json.Serialization;

namespace BananaTracks.Api.Configuration;

[JsonSerializable(typeof(ActivitiesListResponse))]
[JsonSerializable(typeof(RoutinesListResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{ }
