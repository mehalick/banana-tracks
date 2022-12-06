namespace BananaTracks.App.Pages;

public partial class DeleteRoutine : AppComponentBase
{
	[Parameter]
	public string RoutineId { get; set; } = default!;

	private async Task DeleteSubmit()
	{
		await HttpClient.PostAsJsonAsync(ApiRoutes.DeleteRoutine, new DeleteRoutineRequest
		{
			RoutineId = RoutineId
		});

		NavigationManager.NavigateTo("routines/list");
	}
}
