namespace BananaTracks.App.Pages;

public partial class DeleteActivity : AppComponentBase
{
	[Parameter]
	public string ActivityId { get; set; } = default!;

	private async Task DeleteSubmit()
	{
		await ApiClient.DeleteActivity(new() {ActivityId = ActivityId});

		NavigateSuccess("activities/list", "Activity successfully deleted.");
	}
}
