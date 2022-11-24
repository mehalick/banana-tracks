namespace BananaTracks.Api.Endpoints;

internal class HealthCheck : EndpointWithoutRequest
{
	public override void Configure()
	{
		Get(ApiRoutes.HealthCheck);
	}

	public override async Task HandleAsync(CancellationToken ct)
	{
		await SendOkAsync(ct);
	}
}
