using Microsoft.AspNetCore.WebUtilities;

namespace BananaTracks.App.Components;

public partial class Page : AppComponentBase
{
	[Parameter]
	public string Title { get; set; } = default!;

	[Parameter]
	public bool IsLoading { get; set; }

	[Parameter]
	public RenderFragment ChildContent { get; set; } = null!;

	protected MarkupString StatusMessage { get; set; }

	protected override void OnInitialized()
	{
		if (NavigationManager.TryGetQueryString<string>("status.text", out var message))
		{
			ShowSuccess(message);
		}
	}

	private void ShowSuccess(string message)
	{
		StatusMessage = new(DecodeMessage(message));

		//await JsRuntime.InvokeVoidAsync("showToast");
	}

	private static string DecodeMessage(string message)
	{
		var decodedBytes = Base64UrlTextEncoder.Decode(message);

		return System.Text.Encoding.UTF8.GetString(decodedBytes);
	}
}
