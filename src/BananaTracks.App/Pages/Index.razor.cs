using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace BananaTracks.App.Pages;

public partial class Index : AppComponentBase
{
	[Inject]
	protected IConfiguration Configuration { get; set; } = null!;

	[Inject]
	protected IOptions<Version> Version { get; set; } = null!;

	[Inject]
	protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

	[Inject]
	protected ILocalStorageService LocalStorageService { get; set; } = null!;

	private string _version = "8428d6719a53432a4f88e4442a92e97438324df6";
	private IReadOnlyCollection<RoutineModel>? _recentRunRoutines;

	protected override async Task OnInitializedAsync()
	{
		var version = Configuration["Git:Commit"];

		if (!string.IsNullOrWhiteSpace(version))
		{
			_version = version;
		}

		_version = Version.Value.CommitHash;

		var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

		if (authState.User.Identity?.IsAuthenticated != true)
		{
			return;
		}

		_recentRunRoutines = await GetRecentRoutines();
	}

	private async Task<IReadOnlyCollection<RoutineModel>> GetRecentRoutines()
	{
		var response = await ApiClient.ListRoutines();

		var routines = response.Routines
			.OrderByDescending(i => i.LastRunAt)
			.Take(3)
			.ToList();

		return routines;
	}
}
