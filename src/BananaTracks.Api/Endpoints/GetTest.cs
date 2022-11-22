using BananaTracks.Shared;
using FastEndpoints;

namespace BananaTracks.Api.Endpoints;

[HttpGet("GetTest")]
public class GetTest : EndpointWithoutRequest <GetTestResponse>
{
	public override async Task HandleAsync(CancellationToken ct)
	{
		await SendAsync(new(), cancellation: ct);
	}
}