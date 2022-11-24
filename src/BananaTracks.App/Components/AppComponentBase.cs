namespace BananaTracks.App.Components;

public abstract class AppComponentBase : ComponentBase
{
	[Inject]
	private protected HttpClient HttpClient { get; set; } = null!;

	[Inject]
	private protected NavigationManager NavigationManager { get; set; } = null!;
}
