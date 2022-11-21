using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;
using Constructs;

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

		var cloudFrontTarget = RecordTarget.FromAlias(new CloudFrontTarget(cloudFrontDistribution));

		var _ = new ARecord(this, Name("AppARecord"), new ARecordProps
		{
			Zone = zone,
			RecordName = $"app.{DomainName}",
			Target = cloudFrontTarget
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

	private Distribution CreateCloudFrontDistribution(Bucket bucket, Certificate certificate)
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

	private static string Name(string name)
	{
		return $"BananaTracks{name}";
	}
}
