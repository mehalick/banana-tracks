namespace BananaTracks.App.Pages;

public partial class Index : AppComponentBase
{
	[Inject]
	protected IConfiguration Configuration { get; set; } = null!;

	private string? _version;

	protected override void OnInitialized()
	{
		_version = Configuration["Version:ShortSha"];
	}

	protected override async Task OnInitializedAsync()
	{
		await ApiClient.GetHealthCheck();
	}
}
