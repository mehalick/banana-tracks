namespace BananaTracks.App.Pages;

public partial class UpdateRoutine : AppComponentBase
{
	[Parameter]
	public string RoutineId { get; set; } = default!;
	
	private UpdateRoutineRequest? _updateRoutineRequest;
	private List<ActivityModel> _activities = new();

	protected override async Task OnInitializedAsync()
	{
		var response = await ApiClient.GetRoutineById(RoutineId);

		_updateRoutineRequest = new()
		{
			RoutineId = response.Routine.RoutineId,
			Name = response.Routine.Name
		};

		_activities = response.Routine.Activities;
	}

	private async Task OnValidSubmit()
	{
		await ApiClient.UpdateRoutine(_updateRoutineRequest!);

		NavigateSuccess("routines/list", "Routine successfully updated.");
	}
	
	private void AddActivity()
	{
		_activities.Add(new());
	}
	
	private void RemoveActivity(ActivityModel activity)
	{
		_activities.Remove(activity);
	}
}
