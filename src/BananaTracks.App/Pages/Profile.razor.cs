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
	private string? _email;

	protected override async Task OnInitializedAsync()
	{
		var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

		_claims = authState.User.Claims.ToList();

		if (_claims is not null)
		{
			_id = _claims.GetClaim("sub");
			_email = _claims.GetClaim("name");
		}
	}

	private void BeginSignOut(MouseEventArgs _)
	{
		NavigationManager.NavigateToLogout("authentication/logout");
	}
}
