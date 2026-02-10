using System;
using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using Microsoft.Extensions.Configuration;
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

// GitHub Models endpoint
var endpoint = configuration["GITHUB_MODELS_ENDPOINT"] ?? "https://models.github.ai/inference";
var model = "openai/gpt-4o";

Console.WriteLine($"📍 Using endpoint: {endpoint}");
Console.WriteLine($"🔑 Token length: {githubToken.Length} characters");
Console.WriteLine($"🤖 Model: {model}");

// --- OpenAI SDK chat interaction ---
var openAIOptions = new OpenAIClientOptions() { Endpoint = new Uri(endpoint) };
var client = new ChatClient(model, new ApiKeyCredential(githubToken), openAIOptions);

var messages = new List<ChatMessage>()
{
    new SystemChatMessage("You are a helpful AI assistant."),
    new UserChatMessage("What is 25 * 4 + 10?"),
};

var requestOptions = new ChatCompletionOptions()
{
    Temperature = 1.0f,
    TopP = 1.0f,
    MaxOutputTokenCount = 1000
};

Console.WriteLine($"\n💬 User: What is 25 * 4 + 10?");

try
{
    var response = client.CompleteChat(messages, requestOptions);
    Console.WriteLine($"🤖 Assistant (OpenAI SDK): {response.Value.Content[0].Text}");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error communicating with GitHub Models");
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

// --- Direct MathPlugin usage ---
var mathPlugin = new MathPlugin();
string mathExpression = "25 * 4 + 10";
string mathResult = mathPlugin.Calculate(mathExpression);
Console.WriteLine($"\n🧮 MathPlugin: {mathExpression} = {mathResult}");
Console.WriteLine("\n✅ Completed OpenAI SDK chat and MathPlugin tool use demo.");
