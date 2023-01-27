using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;
using BananaTracks.Api.Shared.Constants;
using BananaTracks.Api.Shared.Requests;
using BananaTracks.Api.Shared.Responses;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BananaTracks.Api.Shared.Clients;

public class ApiClient
{
	private readonly HttpClient _httpClient;

	public ApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task GetHealthCheck()
	{
		await Get(ApiRoutes.HealthCheck);
	}

	public async Task AddActivity(AddActivityRequest request)
	{
		await Post(ApiRoutes.AddActivity, request, ApiClientSerializerContext.Default.AddActivityRequest);
	}

	public async Task AddRoutine(AddRoutineRequest request)
	{
		await Post(ApiRoutes.AddRoutine, request, ApiClientSerializerContext.Default.AddRoutineRequest);
	}

	public async Task AddSession(AddSessionRequest request)
	{
		await Post(ApiRoutes.AddSession, request, ApiClientSerializerContext.Default.AddSessionRequest);
	}

	public async Task DeleteActivity(DeleteActivityRequest request)
	{
		await Post(ApiRoutes.DeleteActivity, request, ApiClientSerializerContext.Default.DeleteActivityRequest);
	}

	public async Task DeleteRoutine(DeleteRoutineRequest request)
	{
		await Post(ApiRoutes.DeleteRoutine, request, ApiClientSerializerContext.Default.DeleteRoutineRequest);
	}

	public async Task<GetActivityByIdResponse> GetActivityById(string activityId)
	{
		var url = $"{ApiRoutes.GetActivityById}?ActivityId={activityId}";

		return await Get(url, ApiClientSerializerContext.Default.GetActivityByIdResponse);
	}

	public async Task<GetRoutineByIdResponse> GetRoutineById(string routineId)
	{
		var url = $"{ApiRoutes.GetRoutineById}?RoutineId={routineId}";

		return await Get(url, ApiClientSerializerContext.Default.GetRoutineByIdResponse);
	}

	public async Task<ListActivitiesResponse> ListActivities()
	{
		return await Get(ApiRoutes.ListActivities, ApiClientSerializerContext.Default.ListActivitiesResponse);
	}

	public async Task<ListRoutinesResponse> ListRoutines()
	{
		return await Get(ApiRoutes.ListRoutines, ApiClientSerializerContext.Default.ListRoutinesResponse);
	}

	public async Task<ListSessionsResponse> ListSessions()
	{
		return await Get(ApiRoutes.ListSessions, ApiClientSerializerContext.Default.ListSessionsResponse);
	}

	public async Task UpdateActivity(UpdateActivityRequest request)
	{
		await Post(ApiRoutes.UpdateActivity, request, ApiClientSerializerContext.Default.UpdateActivityRequest);
	}

	public async Task UpdateRoutine(UpdateRoutineRequest request)
	{
		await Post(ApiRoutes.UpdateRoutine, request, ApiClientSerializerContext.Default.UpdateRoutineRequest);
	}

	private async Task Get(string uri)
	{
		try
		{
			await _httpClient.GetAsync(uri);
		}
		catch (AccessTokenNotAvailableException ex)
		{
			ex.Redirect();
		}
	}

	private async Task<T> Get<T>(string uri, JsonTypeInfo<T> typeInfo, [CallerMemberName] string callerName = "") where T : new()
	{
		var result = default(T);

		try
		{
			result = await _httpClient.GetFromJsonAsync(uri, typeInfo);
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

	private async Task Post<T>(string uri, T value, JsonTypeInfo<T> typeInfo) where T : new()
	{
		try
		{
			await _httpClient.PostAsJsonAsync(uri, value, typeInfo);
		}
		catch (AccessTokenNotAvailableException ex)
		{
			ex.Redirect();
		}
	}
}

[JsonSerializable(typeof(AddActivityRequest))]
[JsonSerializable(typeof(AddRoutineRequest))]
[JsonSerializable(typeof(AddSessionRequest))]
[JsonSerializable(typeof(DeleteActivityRequest))]
[JsonSerializable(typeof(DeleteRoutineRequest))]
[JsonSerializable(typeof(GetActivityByIdResponse))]
[JsonSerializable(typeof(GetRoutineByIdResponse))]
[JsonSerializable(typeof(ListActivitiesResponse))]
[JsonSerializable(typeof(ListRoutinesResponse))]
[JsonSerializable(typeof(ListSessionsResponse))]
[JsonSerializable(typeof(UpdateActivityRequest))]
[JsonSerializable(typeof(UpdateRoutineRequest))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class ApiClientSerializerContext : JsonSerializerContext
{ }
