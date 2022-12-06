namespace BananaTracks.Api.Endpoints;

internal class HealthCheck : EndpointWithoutRequest
{
	public override void Configure()
	{
		AllowAnonymous();
		Get(ApiRoutes.HealthCheck);
	}

	public override async Task HandleAsync(CancellationToken cancellationToken)
	{
		await SendOkAsync(cancellationToken);
	}
}
