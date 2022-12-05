using Amazon.SQS;
using BananaTracks.Core.Entities;
using BananaTracks.Core.Messages;

namespace BananaTracks.Api.Endpoints;

internal class AddActivity : Endpoint<AddActivityRequest>
{
	private readonly IConfiguration _configuration;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IDynamoDBContext _dynamoDbContext;
	private readonly IAmazonSQS _sqsClient;

	public override void Configure()
	{
		Post(ApiRoutes.AddActivity);
		SerializerContext(AppJsonSerializerContext.Default);
	}

	public AddActivity(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IDynamoDBContext dynamoDbContext, IAmazonSQS sqsClient)
	{
		_configuration = configuration;
		_httpContextAccessor = httpContextAccessor;
		_dynamoDbContext = dynamoDbContext;
		_sqsClient = sqsClient;
	}

	public override async Task HandleAsync(AddActivityRequest request, CancellationToken cancellationToken)
	{
		var activity = await SaveActivity(request, cancellationToken);

		await SendActivityCreatedMessage(activity, cancellationToken);

		await SendOkAsync(cancellationToken);
	}

	private async Task<Activity> SaveActivity(AddActivityRequest request, CancellationToken cancellationToken)
	{
		var userId = _httpContextAccessor.GetUserId();

		var activity = new Activity
		{
			UserId = userId,
			Name = request.Name
		};

		await _dynamoDbContext.SaveAsync(activity, cancellationToken);
		
		return activity;
	}

	private async Task SendActivityCreatedMessage(Activity activity, CancellationToken cancellationToken)
	{
		var url = _configuration["AWS:SQS:ActivityCreatedQueueUrl"];

		var json = JsonSerializer.Serialize(new ActivityCreatedMessage
		{
			UserId = activity.UserId,
			ActivityId = activity.ActivityId
		});

		await _sqsClient.SendMessageAsync(url, json, cancellationToken);
	}
}
