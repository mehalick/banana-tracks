using BananaTracks.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BananaTracks.App.Pages
{
	public partial class Activities : ComponentBase
	{
		[Inject]
		protected HttpClient Http { get; set; } = null!;

		private GetTestResponse? _response;

		protected override async Task OnInitializedAsync()
		{
			_response = await Http.GetFromJsonAsync<GetTestResponse>("GetActivities");
		}
	}
}
