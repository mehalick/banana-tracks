using System.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace BananaTracks.App.Components;

public abstract class AppComponentBase : ComponentBase
{
	[Inject]
	private protected ApiClient ApiClient { get; set; } = null!;

	[Inject]
	private protected NavigationManager NavigationManager { get; set; } = null!;

	[Inject]
	private protected IJSRuntime JsRuntime { get; set; } = null!;

	private protected Ids Ids { get; set; } = new();

	public void NavigateSuccess(string uri, string message)
	{
		var uriBuilder = new UriBuilder(NavigationManager.ToAbsoluteUri(uri));
		var query = HttpUtility.ParseQueryString(uriBuilder.Query);
		query["status.text"] = EncodeMessage(message);
		query["status.type"] = "success";
		uriBuilder.Query = query.ToString();

		NavigationManager.NavigateTo(uriBuilder.ToString());
	}

	private static string EncodeMessage(string message)
	{
		var encodedBytes = System.Text.Encoding.UTF8.GetBytes(message);

		return Base64UrlTextEncoder.Encode(encodedBytes);
	}
}

public class Ids
{
	private string _id = $"_{Guid.NewGuid():N}";

	public string Current()
	{
		return _id;
	}

	public string Next()
	{
		_id = $"_{Guid.NewGuid():N}";

		return _id;
	}
}
