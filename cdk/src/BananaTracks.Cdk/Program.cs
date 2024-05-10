using Amazon.CDK;
using Microsoft.Extensions.Configuration;

namespace BananaTracks.Cdk;

internal sealed class Program
{
	public static void Main()
	{
		var builder = new ConfigurationBuilder();
		builder.AddUserSecrets<Program>();

		var configuration = builder.Build();

		var app = new App();
		var _ = new CdkStack(app, "BananaTracks", new StackProps
		{
			Env = new Amazon.CDK.Environment
			{
			    Account = configuration["CdkAccount"],
			    Region = configuration["CdkRegion"]
			}
		});

		app.Synth();
	}
}
