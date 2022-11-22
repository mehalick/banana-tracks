using System.Security.Claims;

namespace BananaTracks.Api.Extensions;

internal static class HttpExtensions
{
	public static string? GetUserId(this IHttpContextAccessor httpContextAccessor)
	{
		return httpContextAccessor.HttpContext?.User.Claims.Single(i => i.Type == ClaimTypes.NameIdentifier).Value;
	}
}
