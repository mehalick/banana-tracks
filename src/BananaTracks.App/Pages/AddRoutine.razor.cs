using BananaTracks.Shared.Models;

namespace BananaTracks.App.Pages;

public partial class AddRoutine : AppComponentBase
{
	private readonly AddRoutineRequest _addRoutineRequest = new();
	private ListActivitiesResponse? _listActivitiesResponse;
	private ActivityModel? _activityModel;

	protected override async Task OnInitializedAsync()
	{
		_listActivitiesResponse = await HttpClient.GetFromJsonAsync<ListActivitiesResponse>(ApiRoutes.ListActivities);

		_activityModel = _listActivitiesResponse?.Activities.First()!;

		_addRoutineRequest.Activities.Add(new()
		{
			ActivityId = _activityModel.ActivityId,
			Name = _activityModel.Name,
			DurationInSeconds = 300,
			BreakInSeconds = 30
		});
	}

	private void AddActivity()
	{
		_addRoutineRequest.Activities.Add(new()
		{
			ActivityId = _activityModel!.ActivityId,
			Name = _activityModel.Name,
			DurationInSeconds = 300,
			BreakInSeconds = 30
		});
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
