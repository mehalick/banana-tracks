using System.Globalization;

namespace BananaTracks.Shared;

public record GetTestResponse
{
	public string Name { get; set; } = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
}