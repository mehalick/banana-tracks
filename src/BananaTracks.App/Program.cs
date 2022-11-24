global using BananaTracks.App.Components;
global using BananaTracks.Shared.Requests;
global using BananaTracks.Shared.Responses;
global using BananaTracks.Shared.Constants;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Components;
global using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BananaTracks.App;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		builder.Services.AddTransient<CustomAuthorizationMessageHandler>();

		builder.Services
			.AddHttpClient("BananaTracks.Api", client => client.BaseAddress = new(builder.Configuration["Api:BaseAddress"]!))
			.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

		builder.Services.AddScoped(sp => sp
			.GetRequiredService<IHttpClientFactory>()
			.CreateClient("BananaTracks.Api"));

		builder.Services.AddOidcAuthentication(options =>
		{
			builder.Configuration.Bind("Auth0", options.ProviderOptions);
			options.ProviderOptions.ResponseType = "code";
			options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
		});

		await builder.Build().RunAsync();
	}
}

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
	public CustomAuthorizationMessageHandler(IConfiguration configuration, IAccessTokenProvider provider, NavigationManager navigation)
		: base(provider, navigation)
	{
		ConfigureHandler(new[] { configuration["Api:BaseAddress"] });
	}
}
