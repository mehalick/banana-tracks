using System.Security.Claims;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using BananaTracks.Api.Entities;
using BananaTracks.Api.Extensions;
using BananaTracks.Shared;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace BananaTracks.Api.Endpoints;

[AllowAnonymous]
[HttpGet("GetActivities")]
public class GetActivities : EndpointWithoutRequest<GetTestResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;
	
	public GetActivities(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var items = await _dynamoDbContext
			.QueryAsync<Activity>(userId)
			.GetRemainingAsync(cancellationToken);

		var activities = items.Select(Activity.Create);
		
		await SendAsync(new(activities), cancellation: cancellationToken);
	}
}