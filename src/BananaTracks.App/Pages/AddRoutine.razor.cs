namespace BananaTracks.App.Pages;

public partial class AddRoutine : AppComponentBase
{
	private readonly AddRoutineRequest _addRoutineRequest = new();
	private ListActivitiesResponse? _listActivitiesResponse;
	private readonly RoutineActivityModel _newActivity = new();

	protected override async Task OnInitializedAsync()
	{
		_listActivitiesResponse = await HttpClient.GetFromJsonAsync<ListActivitiesResponse>(ApiRoutes.ListActivities);
	}

	private async Task AddActivity()
	{
		_addRoutineRequest.Activities.Add(new()
		{
			ActivityId = _newActivity.ActivityId,
			Name = _newActivity.Name,
			DurationInSeconds = _newActivity.DurationInSeconds,
			BreakInSeconds = _newActivity.BreakInSeconds
		});

		await Task.Delay(100);

		await JsRuntime.InvokeVoidAsync("blurActive");
	}

	private void RemoveActivity(RoutineActivityModel activity)
	{
		_addRoutineRequest.Activities.Remove(activity);
	}

	public async Task OnValidSubmit()
	{
		foreach (var activity in _addRoutineRequest.Activities)
		{
			activity.Name = _listActivitiesResponse!.Activities.First(i => i.ActivityId == activity.ActivityId).Name;
		}

		await HttpClient.PostAsJsonAsync(ApiRoutes.AddRoutine, _addRoutineRequest);

		NavigationManager.NavigateTo("routines/list");
	}
}
