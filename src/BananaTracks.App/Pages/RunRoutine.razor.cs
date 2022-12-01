using BananaTracks.Shared.Models;
using Humanizer;
using Microsoft.JSInterop;
using TimeSpan = System.TimeSpan;

namespace BananaTracks.App.Pages;

public partial class RunRoutine : AppComponentBase, IDisposable
{
	private class ActivityRun
	{
		public RoutineActivityModel Activity { get; }
		public bool IsCurrent { get; private set; }
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

		public async Task<bool> UpdateDuration(Func<Task> onComplete)
		{
			_durationStart ??= DateTime.Now;

			Console.WriteLine(_durationStart.Value);

			IsCurrent = true;
			DurationRemaining = _durationTime.Subtract(DateTime.Now.Subtract(_durationStart.Value));

			Console.WriteLine(DurationRemaining);
			Console.WriteLine(DurationRemaining.Ticks);

			if (DurationRemaining.Ticks <= 0)
			{
				await onComplete();
				return true;
			}

			return false;
		}

		public async Task<bool> UpdateBreak(Func<Task> onComplete)
		{
			if (_breakTime == TimeSpan.Zero)
			{
				IsCurrent = false;
				return true;
			}

			_breakState ??= DateTime.Now;

			BreakRemaining = _breakTime.Subtract(DateTime.Now.Subtract(_breakState.Value));

			if (BreakRemaining <= TimeSpan.Zero)
			{
				IsCurrent = false;
				await onComplete();
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

			while (await _timer.WaitForNextTickAsync())
			{
				var isComplete = await activity.UpdateDuration(async () =>
					await JsRuntime.InvokeAsync<string>("playAudio", "dingAudio"));

				await InvokeAsync(StateHasChanged);

				if (isComplete)
				{
					break;
				}
			}

			while (await _timer.WaitForNextTickAsync())
			{
				var isComplete = await activity.UpdateBreak(async () =>
					await JsRuntime.InvokeAsync<string>("playAudio", "dingAudio"));

				await InvokeAsync(StateHasChanged);

				if (isComplete)
				{
					break;
				}
			}
		}

		_runStatus = RunStatus.IsDone;
	}

	private static string DisplayTime(TimeSpan timeSpan)
	{
		if (timeSpan <= TimeSpan.Zero)
		{
			return "";
		}
		
		var ts = TimeSpan.FromSeconds(Math.Round(timeSpan.TotalSeconds));

		return $"{ts.Minutes:00}:{ts.Seconds:00}";
	}

	public void Dispose() => _timer.Dispose();
}
