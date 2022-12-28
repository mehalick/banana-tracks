namespace BananaTracks.App.Pages;

public partial class UpdateActivity : AppComponentBase
{
	[Parameter]
	public string ActivityId { get; set; } = default!;
	
	private UpdateActivityRequest? _updateActivityRequest;

	protected override async Task OnInitializedAsync()
	{
		var activity = await ApiClient.GetActivityById(ActivityId);

		_updateActivityRequest = new()
		{
			ActivityId = activity.Activity.ActivityId,
			Name = activity.Activity.Name
		};
	}

	public async Task OnValidSubmit()
	{
		await ApiClient.UpdateActivity(_updateActivityRequest!);

		NavigateSuccess("activities/list", "Activity successfully updated.");
	}
}
