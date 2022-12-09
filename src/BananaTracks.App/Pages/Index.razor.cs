using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BananaTracks.App.Pages;

public partial class Index : AppComponentBase
{
	[Inject]
	protected IConfiguration Configuration { get; set; } = null!;

	private string? _gitHash;

	protected override void OnInitialized()
	{
		_gitHash = Configuration["Version:GitHash"];
	}

	protected override async Task OnInitializedAsync()
	{
		try
		{
			await HttpClient.GetAsync(ApiRoutes.HealthCheck);
		}
		catch (AccessTokenNotAvailableException)
		{
			// do nothing
		}
	}
}
