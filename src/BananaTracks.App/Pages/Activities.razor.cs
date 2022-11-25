namespace BananaTracks.App.Pages;

[Authorize]
public partial class Activities : AppComponentBase
{
	private ListActivitiesResponse? _response;

	protected override async Task OnInitializedAsync()
	{
		_response = await HttpClient.GetFromJsonAsync<ListActivitiesResponse>(ApiRoutes.ListActivities);
	}
}
