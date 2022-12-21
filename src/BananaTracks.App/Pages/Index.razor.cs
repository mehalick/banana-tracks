namespace BananaTracks.App.Pages;

public partial class Index : AppComponentBase
{
	[Inject]
	protected IConfiguration Configuration { get; set; } = null!;

	private string _version = "8428d6719a53432a4f88e4442a92e97438324df6";

	protected override void OnInitialized()
	{
		var version = Configuration["Git:Commit"];

		if (!string.IsNullOrWhiteSpace(version))
		{
			_version = version;
		}
	}

	protected override async Task OnInitializedAsync()
	{
		await ApiClient.GetHealthCheck();
	}
}
