using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Security.Claims;

namespace BananaTracks.App.Pages;

public partial class Profile : AppComponentBase
{
	[Inject]
	protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

	private List<Claim>? _claims;
	private string? _id;
	private string? _name;
	private ListSessionsResponse? _listSessionsResponse;
	private int _timezoneOffset;

	protected override async Task OnInitializedAsync()
	{
		var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

		_claims = authState.User.Claims.ToList();

		if (_claims is not null)
		{
			_id = _claims.GetClaim("sub");
			_name = _claims.GetClaim("name");
		}

		_timezoneOffset = await JsRuntime.InvokeAsync<int>("BananaTracks.App.getTimezoneOffset");

		_listSessionsResponse = await ApiClient.ListSessions();
	}

	private void LogOut(MouseEventArgs _)
	{
		NavigationManager.NavigateToLogout("authentication/logout");
	}
}
