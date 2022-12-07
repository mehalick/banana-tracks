using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BananaTracks.App.Pages;

public partial class Index : AppComponentBase
{
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
