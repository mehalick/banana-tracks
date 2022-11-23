using BananaTracks.Shared;
using System.Text.Json.Serialization;

namespace BananaTracks.Api.Configuration;

[JsonSerializable(typeof(GetActivitiesResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{ }
