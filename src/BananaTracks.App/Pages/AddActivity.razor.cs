namespace BananaTracks.App.Pages;

public partial class AddActivity : AppComponentBase
{
	private readonly AddActivityRequest _addActivityRequest = new();

	public async Task OnValidSubmit()
	{
		await ApiClient.AddActivity(_addActivityRequest);

		NavigateSuccess("activities/list", "Activity successfully added.");
	}
}
