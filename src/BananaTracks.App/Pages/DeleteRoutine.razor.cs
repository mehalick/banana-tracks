namespace BananaTracks.App.Pages;

public partial class DeleteRoutine : AppComponentBase
{
	[Parameter]
	public string RoutineId { get; set; } = default!;

	private async Task DeleteSubmit()
	{
		await ApiClient.DeleteRoutine(new() {RoutineId = RoutineId});

		NavigateSuccess("routines/list", "Routine successfully deleted.");
	}
}
