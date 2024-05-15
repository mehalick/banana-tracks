global using BananaTracks.Api.Shared.Clients;
global using BananaTracks.Api.Shared.Models;
global using BananaTracks.Api.Shared.Requests;
global using BananaTracks.Api.Shared.Responses;
global using BananaTracks.App.Components;
global using BananaTracks.App.Extensions;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Components;
global using Microsoft.JSInterop;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Linq;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BananaTracks.App;

internal static class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		builder.Services.Configure<Version>(builder.Configuration.GetSection("Version"));

		builder.Services.AddTransient<CustomAuthorizationMessageHandler>();

		builder.Services
			.AddHttpClient<ApiClient>(client => client.BaseAddress = new(builder.Configuration["Api:BaseAddress"]!))
			.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

		builder.Services.AddOidcAuthentication(options =>
		{
			options.ProviderOptions.Authority = builder.Configuration["AWS:Cognito:Authority"];
			options.ProviderOptions.ClientId = builder.Configuration["AWS:Cognito:ClientId"];
			options.ProviderOptions.ResponseType = "code";
			options.ProviderOptions.RedirectUri = builder.Configuration["AWS:Cognito:RedirectUri"];
			options.ProviderOptions.PostLogoutRedirectUri = "";
		});

		builder.Services.AddBlazoredLocalStorage();

		await builder.Build().RunAsync();
	}
}

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
	public CustomAuthorizationMessageHandler(IConfiguration configuration, IAccessTokenProvider provider, NavigationManager navigation)
		: base(provider, navigation)
	{
		ConfigureHandler(new[]
		{
			configuration["Api:BaseAddress"]!
		});
	}
}
