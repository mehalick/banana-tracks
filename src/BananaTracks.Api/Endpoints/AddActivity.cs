using Amazon.SQS;
using BananaTracks.Core.Entities;
using BananaTracks.Core.Messages;

namespace BananaTracks.Api.Endpoints;

internal class AddActivity : Endpoint<AddActivityRequest>
{
	private readonly IConfiguration _configuration;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;

	public override void Configure()
	{
		Post(ApiRoutes.AddActivity);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public AddActivity(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext)
	{
		_configuration = configuration;
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

		var url = _configuration["AWS:SQS:ActivityCreatedQueueUrl"];

		if (string.IsNullOrWhiteSpace(url))
		{
			url = "https://sqs.us-east-1.amazonaws.com/856057347702/ActivityCreated";
		}

		var json = JsonSerializer.Serialize(new ActivityCreatedMessage
		{
			UserId = activity.UserId,
			ActivityId = activity.ActivityId
		});

		await client.SendMessageAsync(url, json, cancellationToken);

		await SendOkAsync(cancellationToken);
	}
}
