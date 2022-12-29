global using Amazon.DynamoDBv2.DataModel;
global using BananaTracks.Api.Extensions;
global using BananaTracks.Api.Providers;
global using BananaTracks.Api.Shared.Constants;
global using BananaTracks.Api.Shared.Models;
global using BananaTracks.Api.Shared.Requests;
global using BananaTracks.Api.Shared.Responses;
global using BananaTracks.Domain.Configuration;
global using BananaTracks.Domain.Entities;
global using FastEndpoints;
global using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.SQS;
using Amazon.SimpleSystemsManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BananaTracks.Api;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		AddAuthServices(builder);
		AddAwsServices(builder);
		AddWebServices(builder);

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
