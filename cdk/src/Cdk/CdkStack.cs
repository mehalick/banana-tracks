using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Constructs;

namespace Cdk;

public class CdkStack : Stack
{
	internal CdkStack(Construct scope, string id, IStackProps props) : base(scope, id, props)
	{
		var _ = new Bucket(this, "bananatracks.com", new BucketProps
		{
			BucketName = "bananatracks.com",
			BlockPublicAccess = new(new BlockPublicAccessOptions
			{
				BlockPublicAcls = false
			}),
			PublicReadAccess = true,
			RemovalPolicy = RemovalPolicy.DESTROY,
			WebsiteIndexDocument = "index.html",
			WebsiteErrorDocument = "error.html"
		});
	}
}