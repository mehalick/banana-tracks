namespace BananaTracks.App.Pages;

public partial class StartRoutine : AppComponentBase, IDisposable
{
	[Parameter]
	public string RoutineId { get; set; } = default!;

	private RoutineRun? _routineRun;

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

		if (response is null)
		{
			throw new NullReferenceException($"Cannot find routine for ID '{RoutineId}'");
		}

		_routineRun = new(response.Routine);
	}

	private async Task RunTimer()
	{
		if (_routineRun is null)
		{
			return;
		}

		await _routineRun.Run(JsRuntime, async () => await InvokeAsync(StateHasChanged));
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

	public void Dispose() => _routineRun?.Dispose();

	private class RoutineRun : IDisposable
	{
		public string Name { get; }
		public RunStatus Status { get; private set; } = RunStatus.IsPending;
		public List<ActivityRun> Activities { get; }

		private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(1));

		public RoutineRun(RoutineModel routine)
		{
			Name = routine.Name;
			Activities = routine.Activities.Select(ActivityRun.Create).ToList();
		}

		private void Start()
		{
			Console.WriteLine($"[RunTimer] Start routine: {Name}");

			Status = RunStatus.IsRunning;
		}

		public async Task Run(IJSRuntime jsRuntime, Func<Task> onUpdate)
		{
			Start();

			foreach (var activity in Activities)
			{
				await activity.StartActivity(jsRuntime);

				while (await _timer.WaitForNextTickAsync())
				{
					var isComplete = await activity.UpdateDuration(jsRuntime);

					await onUpdate();

					if (isComplete)
					{
						break;
					}
				}

				while (await _timer.WaitForNextTickAsync())
				{
					var isComplete = activity.UpdateBreak();

					await onUpdate();

					if (isComplete)
					{
						break;
					}
				}
			}

			await Complete(jsRuntime);
		}

		private async Task Complete(IJSRuntime jsRuntime)
		{
			Status = RunStatus.IsDone;

			Console.WriteLine("[RunTimer] Play audio #audio-done");

			await jsRuntime.InvokeAsync<string>("playAudio", "audio-done");
		}

		public void Dispose() => _timer.Dispose();
	}

	private class ActivityRun
	{
		public RoutineActivityModel Activity { get; }
		public ActivityStatus Status { get; private set; } = ActivityStatus.IsPending;
		public TimeSpan DurationRemaining { get; private set; }
		public TimeSpan BreakRemaining { get; private set; }

		private readonly TimeSpan _durationTime;
		private readonly TimeSpan _breakTime;
		private DateTime? _durationStart;
		private DateTime? _breakState;

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

		public async Task StartActivity(IJSRuntime jsRuntime)
		{
			Console.WriteLine($"[RunTimer] Start activity: {Activity.Name}");

			Status = ActivityStatus.IsRunning;

			if (!string.IsNullOrWhiteSpace(Activity.AudioUrl))
			{
				Console.WriteLine($"[RunTimer] Play audio #audio-{Activity.ActivityId}");

				await jsRuntime.InvokeAsync<string>("playAudio", $"audio-{Activity.ActivityId}");
			}
		}

		public async Task<bool> UpdateDuration(IJSRuntime jsRuntime)
		{
			_durationStart ??= DateTime.Now;

			DurationRemaining = _durationTime.Subtract(DateTime.Now.Subtract(_durationStart.Value));

			if (DurationRemaining.Ticks > 0)
			{
				return false;
			}

			if (_breakTime == TimeSpan.Zero)
			{
				return true;
			}

			Console.WriteLine("[RunTimer] Play audio #audio-break");

			await jsRuntime.InvokeAsync<string>("playAudio", "audio-break");

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

			if (BreakRemaining > TimeSpan.Zero)
			{
				return false;
			}

			Status = ActivityStatus.IsDone;

			return true;
		}
	}
}
