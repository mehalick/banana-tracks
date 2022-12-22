namespace BananaTracks.App.Components;

public partial class LinkList : ComponentBase
{
	[Parameter]
	public RenderFragment ChildContent { get; set; } = null!;
}
