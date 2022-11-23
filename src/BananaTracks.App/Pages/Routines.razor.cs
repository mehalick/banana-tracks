namespace BananaTracks.App.Pages;

[Authorize]
public partial class Routines : AppComponentBase
{
	private RoutinesListResponse? _response;

	protected override async Task OnInitializedAsync()
	{
		_response = await HttpClient.GetFromJsonAsync<RoutinesListResponse>(ApiRoutes.RoutinesList);
	}
}
