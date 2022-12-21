using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace BananaTracks.App.Services;

internal class ApiClient
{
	private readonly HttpClient _httpClient;

	public ApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<ListRoutinesResponse> ListRoutines()
	{
		return await Get(() => _httpClient.GetFromJsonAsync(ApiRoutes.ListRoutines, AppJsonSerializerContext.Default.ListRoutinesResponse));
	}

	private static async Task<T> Get<T>(Func<Task<T?>> action, [CallerMemberName] string callerName = "") where T : new()
	{
		var result = default(T);

		try
		{
			result = await action();
		}
		catch (AccessTokenNotAvailableException ex)
		{
			ex.Redirect();
		}

		if (result is null)
		{
			throw new NullReferenceException($"Null result returned from API call '{callerName}'.");
		}

		return result;
	}
}

[JsonSerializable(typeof(AddActivityRequest))]
[JsonSerializable(typeof(AddRoutineRequest))]
[JsonSerializable(typeof(AddSessionRequest))]
[JsonSerializable(typeof(DeleteActivityRequest))]
[JsonSerializable(typeof(DeleteRoutineRequest))]
[JsonSerializable(typeof(GetRoutineByIdResponse))]
[JsonSerializable(typeof(ListActivitiesResponse))]
[JsonSerializable(typeof(ListRoutinesResponse))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{ }
