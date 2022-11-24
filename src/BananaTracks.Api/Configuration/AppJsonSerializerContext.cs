using BananaTracks.Api.Endpoints.Activities;
using System.Text.Json.Serialization;
using BananaTracks.Shared.Requests;

namespace BananaTracks.Api.Configuration;

[JsonSerializable(typeof(ActivityAddRequest))]
[JsonSerializable(typeof(ActivitiesListResponse))]
[JsonSerializable(typeof(RoutinesListResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{ }
