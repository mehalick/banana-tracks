using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BananaTracks.App.Pages;

public partial class Index : AppComponentBase
{
	[Inject]
	protected IConfiguration Configuration { get; set; } = null!;

	private string? _version;

	protected override void OnInitialized()
	{
		_version = $"{Configuration["Version:ShortSha"]}|{Configuration["Version:RunId"]}" ;
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
