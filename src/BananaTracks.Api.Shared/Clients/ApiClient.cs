using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;
using BananaTracks.Api.Shared.Constants;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BananaTracks.Api.Shared.Clients;

public class ApiClient
{
	private readonly HttpClient _httpClient;

	public ApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task AddActivity(AddActivityRequest request)
	{
		await Post(ApiRoutes.AddActivity, request, ApiSerializer.Default.AddActivityRequest);
	}

	public async Task AddRoutine(AddRoutineRequest request)
	{
		await Post(ApiRoutes.AddRoutine, request, ApiSerializer.Default.AddRoutineRequest);
	}

	public async Task AddSession(AddSessionRequest request)
	{
		await Post(ApiRoutes.AddSession, request, ApiSerializer.Default.AddSessionRequest);
	}

	public async Task<CreateRoutineResponse> CreateRoutine(CreateRoutineRequest request)
	{
		return await Post(ApiRoutes.CreateRoutine, request, ApiSerializer.Default.CreateRoutineRequest, ApiSerializer.Default.CreateRoutineResponse);
	}

	public async Task DeleteActivity(DeleteActivityRequest request)
	{
		await Post(ApiRoutes.DeleteActivity, request, ApiSerializer.Default.DeleteActivityRequest);
	}

	public async Task DeleteRoutine(DeleteRoutineRequest request)
	{
		await Post(ApiRoutes.DeleteRoutine, request, ApiSerializer.Default.DeleteRoutineRequest);
	}

	public async Task<GetActivityByIdResponse> GetActivityById(string activityId)
	{
		var url = $"{ApiRoutes.GetActivityById}?ActivityId={activityId}";

		return await Get(url, ApiSerializer.Default.GetActivityByIdResponse);
	}

	public async Task<GetRoutineByIdResponse> GetRoutineById(string routineId)
	{
		var url = $"{ApiRoutes.GetRoutineById}?RoutineId={routineId}";

		return await Get(url, ApiSerializer.Default.GetRoutineByIdResponse);
	}

	public async Task<ListActivitiesResponse> ListActivities()
	{
		return await Get(ApiRoutes.ListActivities, ApiSerializer.Default.ListActivitiesResponse);
	}

	public async Task<ListRoutinesResponse> ListRoutines()
	{
		return await Get(ApiRoutes.ListRoutines, ApiSerializer.Default.ListRoutinesResponse);
	}

	public async Task<ListSessionsResponse> ListSessions()
	{
		return await Get(ApiRoutes.ListSessions, ApiSerializer.Default.ListSessionsResponse);
	}

	public async Task UpdateActivity(UpdateActivityRequest request)
	{
		await Post(ApiRoutes.UpdateActivity, request, ApiSerializer.Default.UpdateActivityRequest);
	}

	public async Task UpdateRoutine(UpdateRoutineRequest request)
	{
		await Post(ApiRoutes.UpdateRoutine, request, ApiSerializer.Default.UpdateRoutineRequest);
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

	private async Task Post<T>(string uri, T request, JsonTypeInfo<T> typeInfo) where T : new()
	{
		try
		{
			await _httpClient.PostAsJsonAsync(uri, request, typeInfo);
		}
		catch (AccessTokenNotAvailableException ex)
		{
			ex.Redirect();
		}
	}

	private async Task<TResponse> Post<TRequest, TResponse>(string uri, TRequest request, JsonTypeInfo<TRequest> requestTypeInfo, JsonTypeInfo<TResponse> responseTypeInfo, [CallerMemberName] string callerName = "") where TRequest : new()
	{
		var result = default(TResponse);

		try
		{
			var response = await _httpClient.PostAsJsonAsync(uri, request, requestTypeInfo);

			response.EnsureSuccessStatusCode();

			result = (await response.Content.ReadFromJsonAsync(responseTypeInfo))!;
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
