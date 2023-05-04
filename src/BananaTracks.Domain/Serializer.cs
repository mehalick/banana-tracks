namespace BananaTracks.Domain;

[JsonSerializable(typeof(ActivityUpdatedMessage))]
[JsonSerializable(typeof(SessionSavedMessage))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class Serializer : JsonSerializerContext
{ }
