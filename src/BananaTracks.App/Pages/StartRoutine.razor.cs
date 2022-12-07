namespace BananaTracks.App.Pages;

public partial class StartRoutine : AppComponentBase, IDisposable
{
	private class ActivityRun
	{
		public RoutineActivityModel Activity { get; }
		public ActivityStatus Status { get; private set; } = ActivityStatus.IsPending;
		public TimeSpan DurationRemaining { get; private set; }
		public TimeSpan BreakRemaining { get; private set; }

		private readonly TimeSpan _durationTime;
		private readonly TimeSpan _breakTime;

		private ActivityRun(RoutineActivityModel routineActivity)
		{
			Activity = routineActivity;

			_durationTime = TimeSpan.FromSeconds(routineActivity.DurationInSeconds);
			_breakTime = TimeSpan.FromSeconds(routineActivity.BreakInSeconds);

			DurationRemaining = _durationTime;
			BreakRemaining = _breakTime;
		}

		public static ActivityRun Create(RoutineActivityModel activity)
		{
			return new(activity);
		}

		private DateTime? _durationStart;
		private DateTime? _breakState;

		public async Task<bool> UpdateDuration(IJSRuntime jsRuntime)
		{
			_durationStart ??= DateTime.Now;

			Status = ActivityStatus.IsRunning;
			DurationRemaining = _durationTime.Subtract(DateTime.Now.Subtract(_durationStart.Value));

			if (DurationRemaining.Ticks > 0)
			{
				return false;
			}
			
			if (_breakTime != TimeSpan.Zero)
			{
				await jsRuntime.InvokeAsync<string>("playAudio", "audio-break");
			}
				
			return true;
		}

		public bool UpdateBreak()
		{
			Status = ActivityStatus.IsBreaking;

			if (_breakTime == TimeSpan.Zero)
			{
				Status = ActivityStatus.IsDone;
				return true;
			}

			_breakState ??= DateTime.Now;

			BreakRemaining = _breakTime.Subtract(DateTime.Now.Subtract(_breakState.Value));

			if (BreakRemaining <= TimeSpan.Zero)
			{
				Status = ActivityStatus.IsDone;
				return true;
			}

			return false;
		}
	}

	[Parameter]
	public string RoutineId { get; set; } = default!;

	private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(1));
	private RoutineModel? _routine;
	private List<ActivityRun> _activities = new();

	private RunStatus _runStatus = RunStatus.IsPending;

	private enum RunStatus
	{
		IsPending, IsRunning, IsDone
	}

	private enum ActivityStatus
	{
		IsPending, IsRunning, IsBreaking, IsDone
	}

	protected override async Task OnInitializedAsync()
	{
		var response = await HttpClient.GetFromJsonAsync<GetRoutineByIdResponse>($"{ApiRoutes.GetRoutineById}?RoutineId={RoutineId}");

		_routine = response?.Routine!;
		_activities = _routine.Activities.Select(ActivityRun.Create).ToList();
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

		foreach (var activity in _activities)
		{
			Console.WriteLine($"[StartTimer] Activity: {activity.Activity.Name}");

			if (!string.IsNullOrWhiteSpace(activity.Activity.AudioUrl))
			{
				await JsRuntime.InvokeAsync<string>("playAudio", $"audio-{activity.Activity.ActivityId}");
			}

			while (await _timer.WaitForNextTickAsync())
			{
				var isComplete = await activity.UpdateDuration(JsRuntime);

				await InvokeAsync(StateHasChanged);

				if (isComplete)
				{
					break;
				}
			}

			while (await _timer.WaitForNextTickAsync())
			{
				var isComplete = activity.UpdateBreak();

				await InvokeAsync(StateHasChanged);

				if (isComplete)
				{
					break;
				}
			}
		}

		_runStatus = RunStatus.IsDone;

		await JsRuntime.InvokeAsync<string>("playAudio", "audio-done");
	}

	private static string DisplayTime(TimeSpan timeSpan)
	{
		if (timeSpan < TimeSpan.Zero)
		{
			return "00:00";
		}

		var ts = TimeSpan.FromSeconds(Math.Round(timeSpan.TotalSeconds));

		return $"{ts.Minutes:00}:{ts.Seconds:00}";
	}

	public void Dispose() => _timer.Dispose();
}
