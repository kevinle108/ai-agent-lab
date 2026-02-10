Finally resolved about lots of troubleshooting. Takeaways:
- when copilot runs dotnet run, it does not have access to my initialized dotnet secrets - it kept using 16 char test-token value instead of the real one set in my working terminal
    - user and copilot are interacting w/ different terminal sessions
- needed to provide working github example that uses different SDK for it to eventually come up w/ working solution

---
## Summary: Issue and Fix

**The Problem:**

The Semantic Kernel's OpenAI connector (`AddOpenAIChatCompletion()`) was **incompatible with GitHub Models authentication**:

1. **Model ID Issue** - Initially used `"gpt-4o"` instead of `"openai/gpt-4o"` → 404 error
2. **Authentication Issue** - Even with the correct model ID, received persistent **401 Unauthorized** errors
3. **Semantic Kernel Limitation** - The Semantic Kernel wrapper wasn't sending authentication headers in the format GitHub Models expects, despite multiple fixes (custom HttpClient, Bearer token headers, etc.)

**Why It Failed:**
- Semantic Kernel's OpenAI connector uses Azure SDK internally with different authentication mechanisms
- GitHub Models requires authentication in a specific format that the OpenAI SDK handles correctly but Semantic Kernel's wrapper didn't

**The Solution:**

**Switched from Semantic Kernel's wrapper to the OpenAI SDK directly:**

- ❌ Removed: `builder.AddOpenAIChatCompletion()` (Semantic Kernel method)
- ✅ Added: `ChatClient` from OpenAI SDK (v2.0.0) with direct GitHub Models configuration
- ✅ Added: `OpenAI` NuGet package (v2.0.0)

**Key Code Changes:**
```csharp
// Direct OpenAI SDK usage (what works)
var openAIOptions = new OpenAIClientOptions() { Endpoint = new Uri(endpoint) };
var client = new ChatClient(model, new ApiKeyCredential(githubToken), openAIOptions);
var response = client.CompleteChat(messages, requestOptions);
```

**Result:** ✅ Application now successfully authenticates with GitHub Models and retrieves AI responses

**Lesson:** When integrating with GitHub Models, use the **OpenAI SDK directly** rather than through Semantic Kernel's connector layer.