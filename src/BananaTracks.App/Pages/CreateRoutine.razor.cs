namespace BananaTracks.App.Pages;

public partial class CreateRoutine : AppComponentBase
{
	private readonly CreateRoutineRequest _createRoutineRequest = new CreateRoutineRequest();

	private async Task OnValidSubmit()
	{
		var routine = await ApiClient.CreateRoutine(_createRoutineRequest);

		NavigateSuccess($"routines/{routine.RoutineId}/update", "Routine successfully created, add one or more activities to finish.");
	}
}
