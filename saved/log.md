# DEV NOTES:
OpenAI SDK vs Github Models
requires token OpenAI token instead of the GITHUB token

---
User: I am getting the following error from Program.cs, however the same GITHUB_TOKEN is used in #file:ExampleFromGithub-dotnet-secrets.cs and it is works and returns a valid response back. Why is it not working in #file:Program.cs 
```
Unhandled exception. Microsoft.SemanticKernel.HttpOperationException: Incorrect API key provided: github_p*********************************************************************************zyNF. You can find your API key at https://platform.openai.com/account/api-keys.     
Status: 401 (Unauthorized)
ErrorCode: invalid_api_key

Content:
{
    "error": {
        "message": "Incorrect API key provided: github_p*********************************************************************************zyNF. You can find your API key at https://platform.openai.com/account/api-keys.",
        "type": "invalid_request_error",
        "param": null,
        "code": "invalid_api_key"
    }
}
```

GitHub Copilot: The error occurs because the code in Program.cs is using the OpenAI provider in Semantic Kernel, which expects a standard OpenAI API key (starting with sk-...), not a GitHub Copilot/GitHub Models token (starting with github_p...).

- In ExampleFromGithub-dotnet-secrets.cs, the code uses the OpenAI .NET SDK with a custom endpoint (https://models.github.ai/inference) and passes the GitHub token, which works for GitHub Models.
- In Program.cs, kernelBuilder.AddOpenAIChatCompletion(...) is designed for OpenAI's own API, not GitHub Models. It tries to use your GitHub token as an OpenAI key, which fails with a 401 Unauthorized.

To fix this, you need to configure Semantic Kernel to use the GitHub Models endpoint and authentication, not the default OpenAI provider. The Semantic Kernel library supports custom endpoints and headers for this use case.

Would you like me to update Program.cs to use the GitHub Models endpoint correctly with your token?

User: yes

GitHub Copilot: Generating patch (57 lines) in [](file:///c%3A/repos/code-you/2026-ai-class/code-you-hw/Unit%203%20-%20AI%20Orchestration/ai-agent-lab/dotnet-semantickernel/Program.cs)


```

```


Checked [](file:///c%3A/repos/code-you/2026-ai-class/code-you-hw/Unit%203%20-%20AI%20Orchestration/ai-agent-lab/dotnet-semantickernel/Program.cs), no problems found

Ran terminal command: dotnet run

Made changes.

User: error CS1739: The best overload for 'AddOpenAIChatCompletion' does not have a parameter named 'endpoint'

The build failed. Fix the build errors and run again.

GitHub Copilot: Searched codebase for "Semantic Kernel GitHub Models endpoint configuration AddOpenAIChatCompletion", 5 results

The error occurs because the AddOpenAIChatCompletion method in Semantic Kernel does not support an endpoint parameter. This method is only for OpenAI's official API, not for custom endpoints like GitHub Models.

Your working examples use the OpenAI .NET SDK directly, which allows setting a custom endpoint. Semantic Kernel currently does not natively support GitHub Models as a drop-in replacement for OpenAI endpoints via AddOpenAIChatCompletion.

**Solution:**  
To use GitHub Models with Semantic Kernel, you must either:
- Use the OpenAI .NET SDK directly (as in your working examples), or
- Implement a custom connector/service for Semantic Kernel that targets the GitHub Models endpoint.

**Quickest fix:**  
If you want to use GitHub Models, use the code pattern from ExampleFromGithub-dotnet-secrets.cs or ExampleFromGithub-env-terminal.cs, not Semantic Kernel's AddOpenAIChatCompletion.

Would you like me to rewrite Program.cs to use the OpenAI .NET SDK directly (like your working example), so it works with GitHub Models?