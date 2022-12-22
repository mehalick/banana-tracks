namespace BananaTracks.App.Components;

public partial class Header: ComponentBase
{
	[Parameter]
	public string Title { get; set; } = default!;
}
