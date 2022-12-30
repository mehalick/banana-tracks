using Microsoft.AspNetCore.Components.Authorization;

namespace BananaTracks.App.Pages;

public partial class Index : AppComponentBase
{
	[Inject]
	protected IConfiguration Configuration { get; set; } = null!;

	[Inject]
	protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

	private string _version = "8428d6719a53432a4f88e4442a92e97438324df6";
	private IReadOnlyCollection<RoutineModel>? _recentRunRoutines;

	protected override async Task OnInitializedAsync()
	{
		var version = Configuration["Git:Commit"];

		if (!string.IsNullOrWhiteSpace(version))
		{
			_version = version;
		}

		var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

		if (authState.User?.Identity?.IsAuthenticated == true)
		{
			var response = await ApiClient.ListRoutines();

			_recentRunRoutines = response.Routines
				.OrderByDescending(i => i.LastRunAt)
				.Take(3)
				.ToList();
		}
	}
}
