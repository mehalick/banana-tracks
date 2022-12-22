namespace BananaTracks.App.Pages;

public partial class UpdateActivity : AppComponentBase
{
	[Parameter]
	public string ActivityId { get; set; } = default!;
	
	protected UpdateActivityRequest? UpdateActivityRequest;

	protected override async Task OnInitializedAsync()
	{
		var activity = await ApiClient.GetActivityById(ActivityId);

		UpdateActivityRequest = new()
		{
			ActivityId = activity.Activity.ActivityId,
			Name = activity.Activity.Name
		};
	}

	public async Task OnValidSubmit()
	{
		await ApiClient.UpdateActivity(UpdateActivityRequest!);

		NavigationManager.NavigateTo("activities/list");
	}
}
