using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;


using System;

using Microsoft.Extensions.Configuration;

// Build configuration from environment variables and user secrets
var builder = new ConfigurationBuilder()
	.AddEnvironmentVariables()
	.AddUserSecrets(typeof(Program).Assembly, optional: true);

var configuration = builder.Build();

// Try to get the GITHUB_TOKEN from configuration
var githubToken = configuration["GITHUB_TOKEN"];

if (string.IsNullOrWhiteSpace(githubToken))
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("❌ GITHUB_TOKEN not found in environment variables or user secrets.");
	Console.ResetColor();
	Console.WriteLine("Please set the token using an environment variable or 'dotnet user-secrets set GITHUB_TOKEN <token>' 🛠️");
}

else
{
	Console.ForegroundColor = ConsoleColor.Green;
	Console.WriteLine("✅ GITHUB_TOKEN loaded successfully! 🔑");
	Console.ResetColor();

	// Create and configure Semantic Kernel
	var kernelBuilder = Kernel.CreateBuilder();

	kernelBuilder.AddOpenAIChatCompletion(
		modelId: "openai/gpt-4o",
		apiKey: githubToken,
		endpoint: "https://models.github.ai/inference"
	);

	// Build the Semantic Kernel instance
	var kernel = kernelBuilder.Build();

	Console.WriteLine("🤖 Semantic Kernel configured to use GitHub Models endpoint!");
}
