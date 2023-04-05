using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SQS;
using Constructs;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;
using Function = Amazon.CDK.AWS.Lambda.Function;
using FunctionProps = Amazon.CDK.AWS.Lambda.FunctionProps;
using IFunction = Amazon.CDK.AWS.Lambda.IFunction;

namespace Cdk;

public class CdkStack : Stack
{
	private const string DomainName = "bananatracks.com";
	private const string WildcardDomain = $"*.{DomainName}";
	private const string ApiDomainName = $"api.{DomainName}";
	private const string AppDomainName = $"app.{DomainName}";
	private const string CdnDomainName = $"cdn.{DomainName}";
	
	internal CdkStack(Construct scope, string id, IStackProps props) : base(scope, id, props)
	{
		var (appBucket, cdnBucket) = CreateS3Buckets();

		var (hostedZone, certificate) = CreateHostedZoneCertificate();

		var (appDistribution, cdnDistribution) = CreateCloudFrontDistributions(appBucket, cdnBucket, certificate);

		var (activitiesTable, routinesTable, sessionsTable) = CreateDynamoDbTables();

		var (activityUpdatedQueue, sessionSavedQueue) = CreateSqsQueues();

		var lambdaRole = CreateLambdaRole(activitiesTable, routinesTable, sessionsTable, activityUpdatedQueue, sessionSavedQueue, cdnBucket);

		_ = CreateActivityUpdatedFunction(lambdaRole, activityUpdatedQueue);
		_ = CreateSessionSavedFunction(lambdaRole, sessionSavedQueue);

		var apiFunction = CreateApiFunction(lambdaRole);

		var apiGateway = CreateApiGateway(apiFunction, certificate);

		CreateDnsRecords(hostedZone, appDistribution, cdnDistribution, apiGateway);
	}

	private (Queue, Queue) CreateSqsQueues()
	{
		var activityUpdatedQueue = new Queue(this, Name("ActivityUpdatedQueue"), new QueueProps
		{
			QueueName = Name("ActivityUpdated.fifo"),
			Fifo = true,
			DeadLetterQueue = new DeadLetterQueue
			{
				MaxReceiveCount = 1,
				Queue = new Queue(this, Name("ActivityUpdatedDeadLetterQueue"), new QueueProps
				{
					QueueName = Name("ActivityUpdated-dlq.fifo"),
					Fifo = true
				})
			}
		});

		var sessionSavedQueue = new Queue(this, Name("SessionSavedQueue"), new QueueProps
		{
			QueueName = Name("SessionSaved.fifo"),
			Fifo = true,
			DeadLetterQueue = new DeadLetterQueue
			{
				MaxReceiveCount = 1,
				Queue = new Queue(this, Name("SessionSavedDeadLetterQueue"), new QueueProps
				{
					QueueName = Name("SessionSaved-dlq.fifo"),
					Fifo = true
				})
			}
		});

		return (activityUpdatedQueue, sessionSavedQueue);
	}

	private (Bucket, Bucket) CreateS3Buckets()
	{
		var appBucket = new Bucket(this, Name("S3AppBucket"), new BucketProps
		{
			BucketName = AppDomainName,
			BlockPublicAccess = new(new BlockPublicAccessOptions
			{
				BlockPublicAcls = false
			}),
			PublicReadAccess = true,
			RemovalPolicy = RemovalPolicy.DESTROY,
			WebsiteIndexDocument = "index.html",
			WebsiteErrorDocument = "index.html"
		});

		var cdnBucket = new Bucket(this, Name("S3CdnBucket"), new BucketProps
		{
			BucketName = CdnDomainName,
			BlockPublicAccess = new(new BlockPublicAccessOptions
			{
				BlockPublicAcls = false
			}),
			PublicReadAccess = true,
			RemovalPolicy = RemovalPolicy.DESTROY,
			WebsiteIndexDocument = "index.html",
			WebsiteErrorDocument = "index.html"
		});

		cdnBucket.AddToResourcePolicy(new(new PolicyStatementProps
		{
			Sid = "AllowS3",
			Effect = Effect.ALLOW,
			Principals = new IPrincipal[] { new AnyPrincipal() },
			Resources = new[]
			{
				cdnBucket.ArnForObjects("polly"),
				cdnBucket.ArnForObjects("polly/*")
			},
			Actions = new[]
			{
				"s3:PutObject"
			}
		}));

		return (appBucket, cdnBucket);
	}

	private (HostedZone, Certificate) CreateHostedZoneCertificate()
	{
		var hostedZone = new HostedZone(this, Name("HostedZone"), new HostedZoneProps
		{
			ZoneName = DomainName
		});

		var certificate = new Certificate(this, Name("Certificate"), new CertificateProps
		{
			DomainName = DomainName,
			SubjectAlternativeNames = new[] { WildcardDomain },
			Validation = CertificateValidation.FromDns(hostedZone)
		});

		return (hostedZone, certificate);
	}

	private void CreateDnsRecords(IHostedZone zone, IDistribution appDistribution, IDistribution cdnDistribution,
		RestApiBase apiGateway)
	{
		_ = new ARecord(this, Name("AppARecord"), new ARecordProps
		{
			Zone = zone,
			RecordName = AppDomainName,
			Target = RecordTarget.FromAlias(new CloudFrontTarget(appDistribution))
		});

		_ = new ARecord(this, Name("CdnARecord"), new ARecordProps
		{
			Zone = zone,
			RecordName = CdnDomainName,
			Target = RecordTarget.FromAlias(new CloudFrontTarget(cdnDistribution))
		});

		_ = new ARecord(this, Name("ApiARecord"), new ARecordProps
		{
			Zone = zone,
			RecordName = ApiDomainName,
			Target = RecordTarget.FromAlias(new ApiGatewayDomain(apiGateway.DomainName!))
		});
	}

	private (Distribution, Distribution) CreateCloudFrontDistributions(IBucket appBucket, IBucket cdnBucket, ICertificate certificate)
	{
		var identity = new OriginAccessIdentity(this, Name("CloudFrontOriginAccessIdentity"));
		appBucket.GrantRead(identity);
		cdnBucket.GrantRead(identity);

		var appDistribution = new Distribution(this, Name("CloudFrontAppDistribution"), new DistributionProps
		{
			DefaultRootObject = "index.html",
			DefaultBehavior = new BehaviorOptions
			{
				Origin = new S3Origin(appBucket, new S3OriginProps
				{
					OriginAccessIdentity = identity
				}),
				ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS,
				AllowedMethods = AllowedMethods.ALLOW_GET_HEAD_OPTIONS,
				CachePolicy = CachePolicy.CACHING_DISABLED
			},
			Certificate = certificate,
			DomainNames = new[] { AppDomainName },
			HttpVersion = HttpVersion.HTTP2_AND_3
		});

		var cdnDistribution = new Distribution(this, Name("CloudFrontCdnDistribution"), new DistributionProps
		{
			DefaultBehavior = new BehaviorOptions
			{
				Origin = new S3Origin(cdnBucket, new S3OriginProps
				{
					OriginAccessIdentity = identity
				}),
				ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS,
				AllowedMethods = AllowedMethods.ALLOW_GET_HEAD
			},
			Certificate = certificate,
			DomainNames = new[] { CdnDomainName },
			HttpVersion = HttpVersion.HTTP2_AND_3
		});

		return (appDistribution, cdnDistribution);
	}

	private (Table, Table, Table) CreateDynamoDbTables()
	{
		var activitiesTable = new Table(this, Name("DynamoDbActivities"), new TableProps
		{
			TableName = Name("Activities"),
			PartitionKey = new Attribute { Name = "UserId", Type = AttributeType.STRING },
			SortKey = new Attribute { Name = "ActivityId", Type = AttributeType.STRING },
			BillingMode = BillingMode.PAY_PER_REQUEST,
			RemovalPolicy = RemovalPolicy.DESTROY,
			PointInTimeRecovery = true
		});

		var routinesTable = new Table(this, Name("DynamoDbRoutines"), new TableProps
		{
			TableName = Name("Routines"),
			PartitionKey = new Attribute { Name = "UserId", Type = AttributeType.STRING },
			SortKey = new Attribute { Name = "RoutineId", Type = AttributeType.STRING },
			BillingMode = BillingMode.PAY_PER_REQUEST,
			RemovalPolicy = RemovalPolicy.DESTROY,
			PointInTimeRecovery = true
		});

		var sessionsTable = new Table(this, Name("DynamoDbSessions"), new TableProps
		{
			TableName = Name("Sessions"),
			PartitionKey = new Attribute { Name = "UserId", Type = AttributeType.STRING },
			SortKey = new Attribute { Name = "SessionId", Type = AttributeType.STRING },
			BillingMode = BillingMode.PAY_PER_REQUEST,
			RemovalPolicy = RemovalPolicy.DESTROY,
			PointInTimeRecovery = true
		});

		return (activitiesTable, routinesTable, sessionsTable);
	}

	private Role CreateLambdaRole(ITable activitiesTable, ITable routinesTable, ITable sessionsTable, IQueue activityUpdatedQueue, IQueue sessionSavedQueue, IBucket cdnBucket)
	{
		var lambdaRole = new Role(this, Name("LambdaRole"),
			new RoleProps
			{
				RoleName = Name("LambdaRole"),
				AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
			});

		lambdaRole.AddToPolicy(new(
			new PolicyStatementProps
			{
				Sid = "AllowCloudWatch",
				Effect = Effect.ALLOW,
				Resources = new[] { "*" },
				Actions = new[]
				{
					"logs:CreateLogGroup",
					"logs:CreateLogStream",
					"logs:PutLogEvents"
				}
			}));

		lambdaRole.AddToPolicy(new(
			new PolicyStatementProps
			{
				Sid = "AllowXRay",
				Effect = Effect.ALLOW,
				Resources = new[] { "*" },
				Actions = new[]
				{
					"xray:PutTraceSegments",
					"xray:PutTelemetryRecords"
				}
			}));

		lambdaRole.AddToPolicy(new(
			new PolicyStatementProps
			{
				Sid = "AllowSystemsManager",
				Effect = Effect.ALLOW,
				Resources = new[] { "*" },
				Actions = new[]
				{
					"ssm:PutParameter",
					"ssm:GetParametersByPath"
				}
			}));

		lambdaRole.AddToPolicy(new(
			new PolicyStatementProps
			{
				Sid = "AllowPolly",
				Effect = Effect.ALLOW,
				Resources = new[] { "*" },
				Actions = new[]
				{
					"polly:StartSpeechSynthesisTask"
				}
			}));

		lambdaRole.AddToPolicy(new(new PolicyStatementProps
		{
			Sid = "AllowDynamoDb",
			Effect = Effect.ALLOW,
			Resources = new[] { activitiesTable.TableArn, routinesTable.TableArn, sessionsTable.TableArn },
			Actions = new[]
			{
				"dynamodb:DescribeTable",
				"dynamodb:GetItem",
				"dynamodb:PutItem",
				"dynamodb:Query",
				"dynamodb:Scan",
				"dynamodb:UpdateItem"
			}
		}));

		lambdaRole.AddToPolicy(new(new PolicyStatementProps
		{
			Sid = "AllowSqs",
			Effect = Effect.ALLOW,
			Resources = new[] { activityUpdatedQueue.QueueArn, sessionSavedQueue.QueueArn },
			Actions = new[]
			{
				"sqs:ReceiveMessage",
				"sqs:DeleteMessage",
				"sqs:GetQueueAttributes",
				"sqs:SendMessage"
			}
		}));

		lambdaRole.AddToPolicy(new(new PolicyStatementProps
		{
			Sid = "AllowS3",
			Effect = Effect.ALLOW,
			Resources = new[] { cdnBucket.BucketArn },
			Actions = new[]
			{
				"s3:PutObject"
			}
		}));

		return lambdaRole;
	}

	private Function CreateActivityUpdatedFunction(IRole lambdaRole, IQueue activityUpdatedQueue)
	{
		var activityUpdatedFunction = new Function(this, Name("ActivityUpdatedFunction"),
			new FunctionProps
			{
				FunctionName = Name("ActivityUpdatedFunction"),
				Code = Code.FromAsset(@"..\src\BananaTracks.Functions.ActivityUpdated\bin\Release\net6.0"),
				Description = "BananaTracks ActivityUpdated Function",
				Handler = "BananaTracks.Functions.ActivityUpdated::BananaTracks.Functions.ActivityUpdated.Function::FunctionHandler",
				MemorySize = 256,
				Role = lambdaRole,
				Runtime = Runtime.DOTNET_6,
				Timeout = Duration.Seconds(30)
			});

		activityUpdatedFunction.AddEventSource(new SqsEventSource(activityUpdatedQueue));

		return activityUpdatedFunction;
	}

	private Function CreateSessionSavedFunction(IRole lambdaRole, IQueue sessionSavedQueue)
	{
		var activityCreatedFunction = new Function(this, Name("SessionSavedFunction"),
			new FunctionProps
			{
				FunctionName = Name("SessionSavedFunction"),
				Code = Code.FromAsset(@"..\src\BananaTracks.Functions.SessionSaved\bin\Release\net6.0"),
				Description = "BananaTracks SessionSaved Function",
				Handler = "BananaTracks.Functions.SessionSaved::BananaTracks.Functions.SessionSaved.Function::FunctionHandler",
				MemorySize = 256,
				Role = lambdaRole,
				Runtime = Runtime.DOTNET_6,
				Timeout = Duration.Seconds(30)
			});

		activityCreatedFunction.AddEventSource(new SqsEventSource(sessionSavedQueue));

		return activityCreatedFunction;
	}

	private Function CreateApiFunction(IRole lambdaRole)
	{
		return new(this, Name("ApiLambda"),
			new FunctionProps
			{
				FunctionName = Name("ApiLambda"),
				Code = Code.FromAsset(@"..\src\BananaTracks.Api\bin\Release\net6.0"),
				Description = "BananaTracks API",
				Handler = "BananaTracks.Api",
				MemorySize = 256,
				Role = lambdaRole,
				Runtime = Runtime.DOTNET_6,
				Timeout = Duration.Seconds(30),
				Tracing = Tracing.ACTIVE
			});
	}

	private RestApi CreateApiGateway(IFunction function, ICertificate certificate)
	{
		var api = new RestApi(this, Name("ApiGateway"),
			new RestApiProps
			{
				RestApiName = Name("ApiGateway"),
				Description = $"Proxy for {function.FunctionName}",
				DeployOptions = new StageOptions
				{
					StageName = "production"
				},
				EndpointTypes = new[] { EndpointType.REGIONAL },
				DomainName = new DomainNameOptions
				{
					DomainName = $"api.{DomainName}",
					Certificate = certificate
				}
			});

		var apiResource = api.Root.AddResource("{proxy+}");
		apiResource.AddMethod("GET", new LambdaIntegration(function));
		apiResource.AddMethod("POST", new LambdaIntegration(function));
		apiResource.AddMethod("OPTIONS", new LambdaIntegration(function));

		return api;
	}

	private static string Name(string name)
	{
		return $"BananaTracks{name}";
	}
}
