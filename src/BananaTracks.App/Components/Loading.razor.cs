namespace BananaTracks.App.Components;

public partial class Loading : ComponentBase
{
	[Parameter]
	public bool IsLoading { get; set; }

	[Parameter]
	public RenderFragment ChildContent { get; set; } = null!;
}