namespace BananaTracks.App.Pages;

public partial class AddRoutine : AppComponentBase
{
	protected AddRoutineRequest AddRoutineRequest = new();

	public async Task OnValidSubmit()
	{
		await HttpClient.PostAsJsonAsync(ApiRoutes.AddRoutine, AddRoutineRequest);

		NavigationManager.NavigateTo("routines");
	}
}
