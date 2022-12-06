using System.Security.Claims;

namespace BananaTracks.Api.Extensions;

internal static class HttpExtensions
{
	/// <summary>
	/// Gets the user's Auth0 ID from claims.
	/// </summary>
	public static string GetUserId(this IHttpContextAccessor httpContextAccessor)
	{
		return httpContextAccessor.HttpContext?.User.Claims.Single(i => i.Type == ClaimTypes.NameIdentifier).Value!;
	}
}
