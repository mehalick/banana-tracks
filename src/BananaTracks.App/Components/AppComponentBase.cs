namespace BananaTracks.App.Components;

public abstract class AppComponentBase : ComponentBase
{
	[Inject]
	protected HttpClient HttpClient { get; set; } = null!;

	[Inject]
	private protected ApiClient ApiClient { get; set; } = null!;

	[Inject]
	private protected NavigationManager NavigationManager { get; set; } = null!;

	[Inject]
	private protected IJSRuntime JsRuntime { get; set; } = null!;

	private protected Ids Ids { get; set; } = new();
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
