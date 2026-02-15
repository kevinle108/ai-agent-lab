using System;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using DotNetSemanticKernel;

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

// --- Semantic Kernel chat with tool use ---
var builder = Kernel.CreateBuilder();

// GitHub Models endpoint
var endpoint = "https://models.github.ai/inference";
var model = "openai/gpt-4o";

builder.AddOpenAIChatCompletion(model, githubToken, endpoint);

builder.Plugins.AddFromType<MathPlugin>();
var kernel = builder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();
var chatHistory = new ChatHistory();
chatHistory.AddSystemMessage("You are a helpful AI assistant.");
chatHistory.AddUserMessage("What is 25 * 4 + 10?");

var executionSettings = new OpenAIPromptExecutionSettings
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
    Temperature = 1.0f,
    TopP = 1.0f,
    MaxTokens = 1000
};

Console.WriteLine($"\n💬 User: What is 25 * 4 + 10?");

try
{
    var response = await chatService.GetChatMessageContentAsync(
        chatHistory,
        executionSettings,
        kernel
    );
    Console.WriteLine($"🤖 Assistant (SK Tool Use): {response.Content}");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error communicating with GitHub Models or running tool use");
    Console.WriteLine($"   Exception: {ex.GetType().Name}");
    Console.WriteLine($"   Message: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("💡 Troubleshooting:");
    Console.WriteLine("   1. Verify GitHub token is valid");
    Console.WriteLine("   2. Check endpoint: https://models.github.ai/inference");
    Console.WriteLine("   3. Ensure token has GitHub Models access");
    Console.WriteLine();
    Console.WriteLine($"   Full error: {ex}");
}
