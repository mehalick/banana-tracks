global using Amazon.DynamoDBv2.DataModel;
global using BananaTracks.Api.Configuration;
global using BananaTracks.Api.Extensions;
global using BananaTracks.Shared.Constants;
global using BananaTracks.Shared.Requests;
global using BananaTracks.Shared.Responses;
global using FastEndpoints;
global using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BananaTracks.Api;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddFastEndpoints();
		builder.Services.AddHttpContextAccessor();
		builder.Services.AddSwaggerGen();

		AddAuthServices(builder);
		AddAwsServices(builder);

		var app = builder.Build();

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
				c.Authority = builder.Configuration["Auth0:Domain"];
				c.Audience = builder.Configuration["Auth0:Audience"];
				c.TokenValidationParameters = new()
				{
					ValidAudience = builder.Configuration["Auth0:Audience"],
					ValidIssuer = builder.Configuration["Auth0:Domain"]
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
		var region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS:DefaultRegion"]);

		if (builder.Environment.IsProduction())
		{
			builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
			builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
			builder.Services.AddSingleton(() => new AmazonSQSClient(region));
		}
		else
		{
			var accessKeyId = builder.Configuration["AwsAccessKeyId"];
			var secretAccessKey = builder.Configuration["AwsSecretAccessKey"];
			var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);

			builder.Services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(credentials));
			builder.Services.AddSingleton(_ => new AmazonSQSClient(credentials, region));
		}

		builder.Services.AddTransient<IDynamoDBContext, DynamoDBContext>();
	}
}
