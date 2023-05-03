namespace BananaTracks.App.Pages;

public partial class AddRoutine : AppComponentBase
{
	private readonly AddRoutineRequest _addRoutineRequest = new()
	{
		Activities = new()
		{
			new AddRoutineRequestActivity()
		}
	};
	
	private void AddActivity()
	{
		_addRoutineRequest.Activities.Add(new());
	}

	private void RemoveActivity(AddRoutineRequestActivity activity)
	{
		_addRoutineRequest.Activities.Remove(activity);
	}

	private async Task OnValidSubmit()
	{
		await ApiClient.AddRoutine(_addRoutineRequest);

		NavigateSuccess("routines/list", "Routine successfully created.");
	}
}
