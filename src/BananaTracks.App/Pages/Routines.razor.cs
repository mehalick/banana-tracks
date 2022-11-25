namespace BananaTracks.App.Pages;

[Authorize]
public partial class Routines : AppComponentBase
{
	private ListRoutinesResponse? _response;

	protected override async Task OnInitializedAsync()
	{
		_response = await HttpClient.GetFromJsonAsync<ListRoutinesResponse>(ApiRoutes.ListRoutines);
	}
}
