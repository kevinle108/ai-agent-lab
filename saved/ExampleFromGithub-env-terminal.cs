using System;
using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

var endpoint = "https://models.github.ai/inference";

var credential = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrEmpty(credential))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Error: The GITHUB_TOKEN environment variable is not set. Please set it to a valid GitHub token and try again.");
    Console.ResetColor();
    return;
}

var model = "openai/gpt-4o";

var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = new Uri(endpoint)

};

var client = new ChatClient(model, new ApiKeyCredential(credential), openAIOptions);

List<ChatMessage> messages = new List<ChatMessage>()
{
    new SystemChatMessage("You are a helpful assistant."),
    new UserChatMessage("What is the capital of France?"),
};

var requestOptions = new ChatCompletionOptions()
{
    Temperature = 1.0f,
    TopP = 1.0f,
    MaxOutputTokenCount = 1000
};

var response = client.CompleteChat(messages, requestOptions);
System.Console.WriteLine(response.Value.Content[0].Text);