global using BananaTracks.App.Components;
global using BananaTracks.App.Extensions;
global using BananaTracks.Api.Shared.Models;
global using BananaTracks.Api.Shared.Requests;
global using BananaTracks.Api.Shared.Responses;
global using BananaTracks.Api.Shared.Constants;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Components;
global using Microsoft.JSInterop;
global using System.Net.Http.Json;
global using BananaTracks.App.Services;
using BananaTracks.Api.Shared.Protos;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
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

		builder.Services
			.AddHttpClient<ApiClient>(client => client.BaseAddress = new(builder.Configuration["Api:BaseAddress"]!))
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

		builder.Services.AddSingleton(services => new RoutineService.RoutineServiceClient(GetChannel(services)));


		await builder.Build().RunAsync();

		static GrpcChannel GetChannel(IServiceProvider services)
		{
			var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
			var baseUri = "https://localhost:7144";
			return GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
		}
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
