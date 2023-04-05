# BananaTracks

BananaTracks is an open-source, free-to-use web app for creating and running custom practice routines. It doesn't store any personal information nor use cookie or tracking features.

Practice routines can be used for music practice, exercise, and any daily activity practice.

## Web App

[https://app.bananatracks.com](https://app.bananatracks.com)

## Source Code

This repository includes the source code for the web app, API, serverless functions, and infrastructure via the AWS Cloud Development Kit (CDK).

## Architecture

![BananaTracks Architecture](https://cdn.bananatracks.com/assets/architecture-20230405084102.svg)

## Deploying

While the public BananaTracks app is free-to-use, you can also clone this repository and deploy to your own AWS account.

## Clone Repository

```pwsh
git clone https://github.com/mehalick/banana-tracks.git

cd .\banana-tracks
```
## Update Domain Names

The infrastructure is deployed to AWS using the AWS Cloud Development Kit (CDK). This repository includes the BananaTracks DNS records for Route53 and you'll want to update those domain names prior to deploying.

```csharp
private const string DomainName = "bananatracks.com";
private const string WildcardDomain = $"*.{DomainName}";
private const string ApiDomainName = $"api.{DomainName}";
private const string AppDomainName = $"app.{DomainName}";
private const string CdnDomainName = $"cdn.{DomainName}";
```

[https://github.com/mehalick/banana-tracks/blob/main/cdk/src/Cdk/CdkStack.cs#L24](https://github.com/mehalick/banana-tracks/blob/main/cdk/src/Cdk/CdkStack.cs#L24)

Alternatively, you can remove Route53 entirely and simply use the CloudFront distribution domain names that are created for you.

## Install AWS and CDK Tools

If you don't already have the AWS and CDK command line interfaces (CLI) installed and configured you can follow the instructions at:

- [AWS CLI](https://aws.amazon.com/cli/)
- [AWS CDK](https://aws.amazon.com/cdk/)

## Configure AWS Account Name and Region

The CDK project will need your AWS account name and region when bootstrapping and deploying. In the CDK project they are set with .NET user secrets:

```pwsh
# set your AWS account name and region
 
$cdkAccount = "<YOUR AWS ACCOUNT NAME>"
$cdkRegion = "us-east-1"

# navigate to CDK project

cd .\cdk\src\Cdk\

# add .NET user secrets to project
 
dotnet user-secrets init
dotnet user-secrets set "CdkAccount" $cdkAccount
dotnet user-secrets set "CdkRegion" $cdkRegion
```

## Build Solution

The CDK will deploy your infrastructure as well as the .NET Lambda projects so you'll first need to build the solution:

```pwsh
# navigate to solution root

cd ..\..

# build in release configuration

dotnet build -c Release
```

## Bootstrap CDK and Deploy

Finally, you can bootstrap the CDK stack and deploy to your AWS account:

```pwsh
# navigate to CDK project

cd .\cdk\

# bootstrap stack

cdk bootstrap aws://$cdkAccount/$cdkRegion

# review CDK stack

cdk diff

# deploy CDK stack

cdk deploy
```

## API Project App Settings

The API project uses [Auth0](https://auth0.com/) for authentication, you'll need to set up an account and add the details to the API project's [app settings](https://github.com/mehalick/banana-tracks/blob/main/src/BananaTracks.Api/appsettings.json).

You'll also need to set the AWS-specific app settings here as well, specifically the region and SQS URLs.

