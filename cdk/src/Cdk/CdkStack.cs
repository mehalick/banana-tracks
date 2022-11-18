using Amazon.CDK;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.S3;
using Constructs;

namespace Cdk;

public class CdkStack : Stack
{
	internal CdkStack(Construct scope, string id, IStackProps props) : base(scope, id, props)
	{
		var bucket = new Bucket(this, Name("S3Bucket"), new BucketProps
		{
			BucketName = "bananatracks.com",
			BlockPublicAccess = new(new BlockPublicAccessOptions
			{
				BlockPublicAcls = false
			}),
			PublicReadAccess = true,
			RemovalPolicy = RemovalPolicy.DESTROY,
			WebsiteIndexDocument = "index.html",
			WebsiteErrorDocument = "index.html"
		});

		var identity = new OriginAccessIdentity(this, Name("CloudFrontOriginAccessIdentity"));
		bucket.GrantRead(identity);

		var _ = new Distribution(this, Name("CloudFrontDistribution"), new DistributionProps
		{
			DefaultRootObject = "index.html",
			DefaultBehavior = new BehaviorOptions
			{
				Origin = new S3Origin(bucket, new S3OriginProps
				{
					OriginAccessIdentity = identity
				})
			}
		});
	}

	private static string Name(string name)
	{
		return $"BananaTracks{name}";
	}
}
