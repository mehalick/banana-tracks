using System.Security.Claims;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using BananaTracks.Api.Entities;
using BananaTracks.Shared;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace BananaTracks.Api.Endpoints;

[AllowAnonymous]
[HttpGet("GetTest")]
public class GetTest : EndpointWithoutRequest<GetTestResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;
	
	public GetTest(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.HttpContext?.User.Claims.Single(i => i.Type == ClaimTypes.NameIdentifier).Value;

		var items = await _dynamoDbContext
			.QueryAsync<Activity>(userId)
			.GetRemainingAsync(cancellationToken);

		var activities = items.Select(Activity.Create);
		
		await SendAsync(new(activities), cancellation: cancellationToken);
	}
}