global using System.Text.Json;
global using Amazon.DynamoDBv2.DataModel;
global using BananaTracks.Api.Extensions;
global using BananaTracks.Api.Providers;
global using BananaTracks.Api.Shared.Constants;
global using BananaTracks.Api.Shared.Requests;
global using BananaTracks.Api.Shared.Responses;
global using BananaTracks.Domain;
global using BananaTracks.Domain.Entities;
global using BananaTracks.Domain.Extensions;
global using FastEndpoints;
using Amazon.CloudWatchLogs;
using Amazon.DynamoDBv2;
using Amazon.SQS;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;

namespace BananaTracks.Api;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		var client = new AmazonCloudWatchLogsClient();

		builder.Host.UseSerilog((context, services, configuration) =>
		{
			configuration
				.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.Enrich.WithProperty("Version", context.Configuration["Version:RunId"])
				.WriteTo.Console()
				.WriteTo.AmazonCloudWatch(logGroup: "/banana-tracks/serilog", logStreamPrefix: DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"), cloudWatchClient: client);
		});

		AddAuthServices(builder);
		AddAwsServices(builder);
		AddWebServices(builder);

		var app = builder.Build();

		app.UseSerilogRequestLogging();

		app.UseXRay("BananaTracks");

		app.UseCors("ApiCors");

		app.UseSwagger();
		app.UseSwaggerUI();

		app.UseHttpsRedirection();

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseFastEndpoints();

		await app.RunAsync();
	}

	private static void AddAuthServices(WebApplicationBuilder builder)
	{
		builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
			{
				c.Authority = builder.Configuration["AWS:Cognito:Authority"];
				c.TokenValidationParameters = new()
				{
					ValidateIssuerSigningKey = true,
					ValidateAudience = false
				};
			});

		builder.Services.AddCors(options =>
		{
			options.AddPolicy("ApiCors", i => i
				.WithOrigins("https://localhost:7143", "https://app.bananatracks.com")
				.AllowAnyMethod()
				.AllowAnyHeader());
		});
	}

	private static void AddAwsServices(WebApplicationBuilder builder)
	{
		var options = builder.Configuration.GetAWSOptions();

		builder.Services.AddDefaultAWSOptions(options);
		builder.Services.AddAWSService<IAmazonDynamoDB>();
		builder.Services.AddAWSService<IAmazonSQS>();
		builder.Services.AddTransient<IDynamoDBContext, DynamoDBContext>();
		builder.Services.AddTransient<QueueProvider>();

		if (builder.Environment.IsProduction())
		{
			builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
		}

		//builder.Services.AddOpenTelemetryTracing(i => {});

		//Sdk.CreateTracerProviderBuilder()
		//	.AddAWSInstrumentation()
		//	.AddXRayTraceId() // for generating AWS X-Ray compliant trace IDs
		//	.AddOtlpExporter() // default address localhost:4317
		//	.Build();

		//Sdk.SetDefaultTextMapPropagator(new AWSXRayPropagator()); // configure AWS X-Ray propagator

		AWSSDKHandler.RegisterXRayForAllServices();
	}

	private static void AddWebServices(WebApplicationBuilder builder)
	{
		if (builder.Environment.IsProduction())
		{
			builder.Services
				.AddDataProtection()
				.PersistKeysToAWSSystemsManager("/BananaTracks/DataProtection");
		}

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddFastEndpoints();
		builder.Services.AddHttpContextAccessor();
		builder.Services.AddSwaggerGen();
	}
}
