namespace BananaTracks.App.Pages;

public partial class Index : AppComponentBase
{
	protected override async Task OnInitializedAsync()
	{
		await HttpClient.GetAsync(ApiRoutes.HealthCheck);
	}
}
