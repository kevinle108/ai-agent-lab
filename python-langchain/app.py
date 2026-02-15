from dotenv import load_dotenv
import os
from datetime import datetime
from langchain.agents import create_agent
from langchain_core.messages import HumanMessage
from langchain_core.tools import Tool
from langchain_openai import ChatOpenAI


def calculator(expression: str) -> str:
    """Evaluate a mathematical expression and return the result as a string.

    This uses Python's built-in eval() with restricted globals for demo purposes only.
    Do NOT use this on untrusted input in production.
    """
    try:
        # Restrict available builtins for safety in this demo context
        result = eval(expression, {"__builtins__": None}, {})
        return str(result)
    except Exception as e:
        return f"Error: {e}"


def main():
    load_dotenv()
    token = os.getenv("GITHUB_TOKEN")
    if not token:
        print("❌ GITHUB_TOKEN not found.")
        print("   - Create a GitHub Personal Access Token: https://github.com/settings/tokens")
        print("   - Add it to a .env file in this folder with: GITHUB_TOKEN=ghp_xxx")
        print("   - Activate your virtualenv and re-run this script.")
        return
    # create ChatOpenAI client using the GitHub token
    chat = ChatOpenAI(
        model="openai/gpt-4o",
        temperature=0,
        base_url="https://models.github.ai/inference",
        api_key=token,
    )
    # tools available to the agent
    tools = [
        Tool(
            name="Calculator",
            func=calculator,
            description=(
                "Evaluate mathematical expressions. Use this tool when the agent needs to "
                "perform arithmetic or numeric calculations exactly (e.g., '25 * 4 + 10'). "
                "Input should be a plain math expression. Returns the computed result as a string."
            ),
        )
    ]
    print("✅ ChatOpenAI client initialized.")
    print("🚀 Starting app — environment loaded.")
    # simple test invocation (no tools)
    llm = chat
    test_query = "What is 25 * 4 + 10?"
    print(f"💬 Query: {test_query}")
    try:
        response = llm.invoke(test_query)
        content = getattr(response, "content", response)
        print("📝 Response:", content)
    except Exception as e:
        print("⚠️ LLM invocation failed:", e)

    # create an agent with the LLM and tools
    try:
        # Create the agent (returns a CompiledStateGraph)
        agent_executor = create_agent(
            llm,
            tools=tools,
            debug=True,
        )
        print("🧭 Agent initialized.")

        # Invoke the agent with messages format (required for LangGraph agents)
        print("🔁 Invoking agent with query...")
        agent_result = agent_executor.invoke({"messages": [HumanMessage(content=test_query)]})
        
        # Extract the final response from messages
        messages = agent_result.get("messages", [])
        if messages:
            # Get the last message (which should be the final response)
            final_message = messages[-1]
            output = getattr(final_message, "content", str(final_message))
        else:
            output = agent_result

        print("🔎 Agent output:", output)
    except Exception as e:
        print("⚠️ Agent invocation failed:", e)


if __name__ == "__main__":
    main()
