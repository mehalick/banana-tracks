global using Amazon.DynamoDBv2.DataModel;
global using BananaTracks.Api.Configuration;
global using BananaTracks.Api.Entities;
global using BananaTracks.Api.Extensions;
global using BananaTracks.Shared.Responses;
global using BananaTracks.Shared.Constants;
global using FastEndpoints;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BananaTracks.Api;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

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

		if (builder.Environment.IsProduction())
		{
			builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
		}

		builder.Services.AddFastEndpoints();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		builder.Services.AddHttpContextAccessor();

		if (builder.Environment.IsProduction())
		{
			builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
		}
		else
		{
			var awsAccessKeyId = builder.Configuration["AwsAccessKeyId"];
			var awsSecretAccessKey = builder.Configuration["AwsSecretAccessKey"];

			builder.Services.AddSingleton<IAmazonDynamoDB>(_ =>
				new AmazonDynamoDBClient(awsAccessKeyId, awsSecretAccessKey));
		}

		builder.Services.AddTransient<IDynamoDBContext, DynamoDBContext>();

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
}
