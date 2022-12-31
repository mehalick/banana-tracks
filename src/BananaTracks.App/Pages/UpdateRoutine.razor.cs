namespace BananaTracks.App.Pages;

public partial class UpdateRoutine : AppComponentBase
{
	[Parameter]
	public string RoutineId { get; set; } = default!;
	
	private UpdateRoutineRequest? _updateRoutineRequest;

	protected override async Task OnInitializedAsync()
	{
		var activity = await ApiClient.GetRoutineById(RoutineId);

		_updateRoutineRequest = new()
		{
			RoutineId = activity.Routine.RoutineId,
			Name = activity.Routine.Name
		};
	}

	public async Task OnValidSubmit()
	{
		await ApiClient.UpdateRoutine(_updateRoutineRequest!);

		NavigateSuccess("routines/list", "Routine successfully updated.");
	}
}
