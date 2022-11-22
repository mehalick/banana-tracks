using BananaTracks.Shared;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace BananaTracks.Api.Endpoints;

[AllowAnonymous]
[HttpGet("GetTest")]
public class GetTest : EndpointWithoutRequest<GetTestResponse>
{
	public override async Task HandleAsync(CancellationToken ct)
	{
		await SendAsync(new(), cancellation: ct);
	}
}
