namespace BananaTracks.App.Pages;

[Authorize]
public partial class Activities : AppComponentBase
{
	private ActivitiesListResponse? _response;

	protected override async Task OnInitializedAsync()
	{
		_response = await HttpClient.GetFromJsonAsync<ActivitiesListResponse>(ApiRoutes.ActivitiesList);
	}
}
