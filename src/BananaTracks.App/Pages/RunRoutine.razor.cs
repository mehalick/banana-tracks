using BananaTracks.Shared.Models;

namespace BananaTracks.App.Pages;

public partial class RunRoutine : AppComponentBase, IDisposable
{
	[Parameter]
	public string RoutineId { get; set; } = default!;

	private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(1));
	private TimeSpan _timeSince;
	private string _currentActivity = default!;
	private RoutineModel? _routine;
	
	private RunStatus _runStatus = RunStatus.IsPending;

	private enum RunStatus
	{
		IsPending, IsRunning, IsDone
	}

	protected override async Task OnInitializedAsync()
	{
		var response = await HttpClient.GetFromJsonAsync<GetRoutineByIdResponse>($"{ApiRoutes.GetRoutineById}?RoutineId={RoutineId}");

		_routine = response?.Routine;
	}

	private async Task StartTimer()
	{
		_runStatus = RunStatus.IsRunning;

		Console.WriteLine("[StartTimer] Start");
		
		if (_routine is null)
		{
			return;
		}

		Console.WriteLine($"[StartTimer] Routine: {_routine.Name}");

		foreach (var activity in _routine.Activities)
		{
			Console.WriteLine($"[StartTimer] Activity: {activity.Name}");

			_currentActivity = activity.Name;

			var start = DateTime.Now.AddSeconds(activity.DurationInSeconds);

			while (await _timer.WaitForNextTickAsync())
			{
				_timeSince = start.Subtract(DateTime.Now);

				if (_timeSince.TotalSeconds < 0)
				{
					break;
				}

				await InvokeAsync(StateHasChanged);
			}

			if (activity.BreakInSeconds == 0)
			{
				break;
			}

			_currentActivity += " (break)";

			start = DateTime.Now.AddSeconds(activity.BreakInSeconds);

			while (await _timer.WaitForNextTickAsync())
			{
				_timeSince = start.Subtract(DateTime.Now);

				if (_timeSince.TotalSeconds < 0)
				{
					break;
				}

				await InvokeAsync(StateHasChanged);
			}
		}

		_runStatus = RunStatus.IsDone;
	}

	public void Dispose() => _timer.Dispose();
}
