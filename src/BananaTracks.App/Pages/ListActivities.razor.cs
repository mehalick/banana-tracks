namespace BananaTracks.App.Pages;

[Authorize]
public partial class ListActivities : AppComponentBase
{
	private ListActivitiesResponse? _response;

	protected override async Task OnInitializedAsync()
	{
		_response = await ApiClient.ListActivities();
	}

	private static void SelectActivity(ActivityModel activity, bool isSelected)
	{
		activity.IsSelected = isSelected;
	}
}
