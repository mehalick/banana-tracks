namespace BananaTracks.App.Pages;

[Authorize]
public partial class Routines : AppComponentBase
{
	private GetActivitiesResponse? _response;

	protected override async Task OnInitializedAsync()
	{
		_response = await HttpClient.GetFromJsonAsync<GetActivitiesResponse>(ApiRoutes.ActivitiesList);
	}
}
