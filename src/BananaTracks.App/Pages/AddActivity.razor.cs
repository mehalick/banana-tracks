namespace BananaTracks.App.Pages;

public partial class AddActivity : AppComponentBase
{
	protected AddActivityRequest AddActivityRequest = new();

	public async Task OnValidSubmit()
	{
		await HttpClient.PostAsJsonAsync(ApiRoutes.AddActivity, AddActivityRequest);

		NavigationManager.NavigateTo("activities");
	}
}
