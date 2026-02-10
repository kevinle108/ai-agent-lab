

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Services;

class Program
{
    static async Task Main(string[] args)
    {
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
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ GITHUB_TOKEN loaded successfully! 🔑");
        Console.WriteLine(githubToken);
        Console.ResetColor();

        // Create and configure Semantic Kernel for GitHub Models
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddOpenAIChatCompletion(
            modelId: "gpt-4o",
            apiKey: githubToken,
            endpoint: "https://models.github.ai/inference"
        );

        // Build the Semantic Kernel instance
        var kernel = kernelBuilder.Build();

        Console.WriteLine("🤖 Semantic Kernel configured to use GitHub Models endpoint!");

        // Get the chat completion service from the kernel
        var chatService = kernel.GetRequiredService<IChatCompletionService>();

        // Create a ChatHistory object
        var chatHistory = new ChatHistory();

        // Add a user message
        chatHistory.AddUserMessage("What is 25 * 4 + 10?");

        // Get a response from the AI without any function calling
        var response = await chatService.GetChatMessageContentAsync(chatHistory);

        // Print the result
        Console.WriteLine($"AI: {response.Content}");
    }
}
