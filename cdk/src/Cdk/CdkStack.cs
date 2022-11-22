using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.IoT;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;
using Constructs;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;
using Function = Amazon.CDK.AWS.Lambda.Function;
using FunctionProps = Amazon.CDK.AWS.Lambda.FunctionProps;
using IFunction = Amazon.CDK.AWS.Lambda.IFunction;

namespace Cdk;

public class CdkStack : Stack
{
	private const string DomainName = "bananatracks.com";

	internal CdkStack(Construct scope, string id, IStackProps props) : base(scope, id, props)
	{
		var bucket = CreateS3Bucket();

		var zone = new HostedZone(this, Name("HostedZone"), new HostedZoneProps
		{
			ZoneName = DomainName
		});

		var certificate = new Certificate(this, Name("Certificate"), new CertificateProps
		{
			DomainName = DomainName,
			SubjectAlternativeNames = new[] { "*.bananatracks.com" },
			Validation = CertificateValidation.FromDns(zone)
		});

		var cloudFrontDistribution = CreateCloudFrontDistribution(bucket, certificate);

		var dynamoDb = new Table(this, Name("DynamoDbActivities"), new TableProps
		{
			TableName = Name("Activities"),
			PartitionKey = new Attribute{ Name = "UserId", Type = AttributeType.STRING },
			SortKey = new Attribute{ Name = "ActivityId", Type = AttributeType.STRING },
			BillingMode = BillingMode.PAY_PER_REQUEST,
			RemovalPolicy = RemovalPolicy.DESTROY,
			PointInTimeRecovery = true
		});

		var function = CreateApiFunction(dynamoDb);

		var api = CreateApiGateway(function, certificate);

		_ = new ARecord(this, Name("AppARecord"), new ARecordProps
		{
			Zone = zone,
			RecordName = $"app.{DomainName}",
			Target = RecordTarget.FromAlias(new CloudFrontTarget(cloudFrontDistribution))
		});

		_ = new ARecord(this, Name("ApiARecord"), new ARecordProps
		{
			Zone = zone,
			RecordName = $"api.{DomainName}",
			Target = RecordTarget.FromAlias(new ApiGatewayDomain(api.DomainName!))
		});

		
	}

	private Bucket CreateS3Bucket()
	{
		return new(this, Name("S3Bucket"), new BucketProps
		{
			BucketName = DomainName,
			BlockPublicAccess = new(new BlockPublicAccessOptions
			{
				BlockPublicAcls = false
			}),
			PublicReadAccess = true,
			RemovalPolicy = RemovalPolicy.DESTROY,
			WebsiteIndexDocument = "index.html",
			WebsiteErrorDocument = "index.html"
		});
	}

	private Distribution CreateCloudFrontDistribution(IBucket bucket, ICertificate certificate)
	{
		var identity = new OriginAccessIdentity(this, Name("CloudFrontOriginAccessIdentity"));
		bucket.GrantRead(identity);

		return new(this, Name("CloudFrontDistribution"), new DistributionProps
		{
			DefaultRootObject = "index.html",
			DefaultBehavior = new BehaviorOptions
			{
				Origin = new S3Origin(bucket, new S3OriginProps
				{
					OriginAccessIdentity = identity
				}),
				ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS,
				AllowedMethods = AllowedMethods.ALLOW_GET_HEAD
			},
			Certificate = certificate,
			DomainNames = new[] { $"app.{DomainName}" },
			HttpVersion = HttpVersion.HTTP2_AND_3
		});
	}

	private Function CreateApiFunction(ITable activitiesTable)
	{
		var lambdaRole = new Role(this, Name("LambdaRole"),
			new RoleProps
			{
				RoleName = Name("LambdaRole"),
				AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
			});

		//var secret = Secret.FromSecretCompleteArn(this, "MySecret", "arn:aws:secretsmanager:us-east-1:965753389244:secret:adp/rdp-IZ1HRA");
		//secret.GrantRead(lambdaRole);

		lambdaRole.AddToPolicy(new(
			new PolicyStatementProps
			{
				Effect = Effect.ALLOW,
				Resources = new[] { "*" },
				Actions = new[]
				{
					"logs:CreateLogGroup",
					"logs:CreateLogStream",
					"logs:PutLogEvents"
				}
			}));

		lambdaRole.AddToPolicy(new(new PolicyStatementProps
		{
			Effect = Effect.ALLOW,
			Resources = new[] { activitiesTable.TableArn },
			Actions = new[]
			{
				"dynamodb:BatchGetItem",
				"dynamodb:BatchWriteItem",
				"dynamodb:DescribeTable",
				"dynamodb:GetItem",
				"dynamodb:PutItem",
				"dynamodb:Query",
				"dynamodb:Scan",
				"dynamodb:UpdateItem"
			}
		}));

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
				Timeout = Duration.Seconds(30)
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
