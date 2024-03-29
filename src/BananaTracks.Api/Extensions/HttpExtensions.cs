﻿using System.Security.Claims;

namespace BananaTracks.Api.Extensions;

internal static class HttpExtensions
{
	/// <summary>
	/// Gets the user's Auth0 ID from claims.
	/// </summary>
	public static string GetUserId(this IHttpContextAccessor httpContextAccessor)
	{
		var claim = httpContextAccessor.HttpContext?.User.Claims.SingleOrDefault(i => i.Type == ClaimTypes.NameIdentifier);

		if (claim is null)
		{
			throw new("Missing name identifier claim for HTTP context user.");
		}

		return claim.Value;
	}

	public static string GetTraceId(this IHttpContextAccessor httpContextAccessor)
	{
		var id = httpContextAccessor.HttpContext?.TraceIdentifier;

		if (id is null)
		{
			throw new("Missing trace identifier for HTTP request.");
		}

		return id;
	}
}
