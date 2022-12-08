using System.Security.Claims;

namespace BananaTracks.App.Extensions;

internal static class ClaimExtensions
{
	/// <summary>
	/// Gets the user claim by name.
	/// </summary>
	public static string? GetClaim(this IEnumerable<Claim> claims, string name)
	{
		var claim = claims.FirstOrDefault(i => i.Type == name);

		return claim?.Value;
	}
}