#pragma warning disable SKEXP0010
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

// Build configuration from appsettings and user secrets
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets("dotnet-semantickernel-secrets")
    .AddEnvironmentVariables()
    .Build();

// Load and validate GitHub token
var githubToken = configuration["GITHUB_TOKEN"];

if (string.IsNullOrWhiteSpace(githubToken))
{
    Console.WriteLine("❌ Error: GITHUB_TOKEN not found!");
    Console.WriteLine();
    Console.WriteLine("📋 To configure your GitHub token:");
    Console.WriteLine("   1. Set environment variable: set GITHUB_TOKEN=your_token_here");
    Console.WriteLine("   2. Or use user secrets: dotnet user-secrets set GITHUB_TOKEN \"your_token_here\"");
    Console.WriteLine();
    Console.WriteLine("💡 Get a token at: https://github.com/settings/tokens");
    return;
}

Console.WriteLine("✅ GitHub token loaded successfully!");

// Initialize Semantic Kernel with GitHub Models
var builder = Kernel.CreateBuilder();

// Configure OpenAI Chat Completion with GitHub Models endpoint
builder.AddOpenAIChatCompletion(
    modelId: "gpt-4o",
    apiKey: githubToken,
    endpoint: new Uri("https://models.github.ai/inference")
);

var kernel = builder.Build();

// TODO: Add skills/plugins and run your AI agent logic

Console.WriteLine("✅ Semantic Kernel AI agent initialized successfully!");
