using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BananaTracks.App.Pages;

public partial class Index : AppComponentBase
{
	[Inject]
	protected IConfiguration Configuration { get; set; } = null!;

	private string? _version;

	protected override void OnInitialized()
	{
		_version = $"{Configuration["Version:GitHash"]}|{Configuration["Version:RunId"]}|{Configuration["Version:RunNumber"]}" ;
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
