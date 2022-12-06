namespace BananaTracks.App.Pages;

public partial class DeleteActivity : AppComponentBase
{
	[Parameter]
	public string ActivityId { get; set; } = default!;

	private async Task DeleteSubmit()
	{
		await HttpClient.PostAsJsonAsync(ApiRoutes.DeleteActivity, new DeleteActivityRequest
		{
			ActivityId = ActivityId
		});

		NavigationManager.NavigateTo("activities/list");
	}
}
