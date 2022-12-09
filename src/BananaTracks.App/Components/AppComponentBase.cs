namespace BananaTracks.App.Components;

public abstract class AppComponentBase : ComponentBase
{
	[Inject]
	protected HttpClient HttpClient { get; set; } = null!;

	[Inject]
	protected NavigationManager NavigationManager { get; set; } = null!;

	[Inject]
	protected IJSRuntime JsRuntime { get; set; } = null!;
}
