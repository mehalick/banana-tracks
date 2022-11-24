using System.Globalization;

namespace BananaTracks.App.Pages;

public partial class AddActivity : AppComponentBase
{
	private protected ActivityAddRequest ActivityAddRequest = new ActivityAddRequest
	{
		Name = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
	};

	public async Task OnValidSubmit()
	{
		await HttpClient.PostAsJsonAsync(ApiRoutes.ActivitiesAdd, ActivityAddRequest);

		NavigationManager.NavigateTo("activities");
	}
}
