﻿using Amazon.SQS;
using BananaTracks.Core.Topics;

namespace BananaTracks.Api.Endpoints;

internal class AddActivity : Endpoint<AddActivityRequest>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Post(ApiRoutes.AddActivity);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public AddActivity(IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
	}

	public override async Task HandleAsync(AddActivityRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var activity = new Activity
		{
			UserId = userId,
			Name = request.Name
		};

		await _dynamoDbContext.SaveAsync(activity, cancellationToken);

		var client = new AmazonSQSClient(Amazon.RegionEndpoint.USEast1);

		var json = JsonSerializer.Serialize(new ActivityCreatedMessage
		{
			UserId = activity.UserId,
			ActivityId = activity.ActivityId
		});

		await client.SendMessageAsync("url", json, cancellationToken);

		await SendOkAsync(cancellationToken);
	}
}
