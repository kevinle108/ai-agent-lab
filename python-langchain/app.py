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



def reverse_string(s: str) -> str:
    """Reverses the input string and returns it.
    
    Args:
        s: The string to reverse.
    
    Returns:
        The reversed string.
    """
    return s[::-1]

def get_current_time(input_str: str) -> str:
    """Returns the current date and time as a formatted string.
    
    Args:
        input_str: A string parameter (required by Tool interface)
    
    Returns:
        The current date and time formatted as YYYY-MM-DD HH:MM:SS
    """
    return datetime.now().strftime("%Y-%m-%d %H:%M:%S")


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
        ),
        Tool(
            name="get_current_time",
            func=get_current_time,
            description=(
                "Get the current date and time. Use this tool when the agent needs to know "
                "what time it is right now. Returns the current date and time in YYYY-MM-DD HH:MM:SS format."
            ),
        ),
        Tool(
            name="reverse_string",
            func=reverse_string,
            description="Reverses a string. Input should be a single string."
        )
    ]
    print("✅ ChatOpenAI client initialized.")
    print("🚀 Starting app — environment loaded.")
    # create an agent with the LLM and tools
    llm = chat
    try:
        agent_executor = create_agent(
            llm,
            tools=tools,
            # system_message="You are a professional and succinct AI assistant. Always answer clearly and concisely.",
            debug=False,
        )
        print("🧭 Agent initialized.")

        test_queries = [
            "What time is it right now?",
            "What is 25 * 4 + 10?",
            "Reverse the string 'Hello World'"
        ]
        print("\nRunning example queries:\n")
        for query in test_queries:
            print("📝 Query:", query)
            print("─" * 50)
            try:
                agent_result = agent_executor.invoke({"messages": [HumanMessage(content=query)]})
                messages = agent_result.get("messages", [])
                if messages:
                    final_message = messages[-1]
                    output = getattr(final_message, "content", str(final_message))
                else:
                    output = agent_result
                print("✅ Result:", output)
            except Exception as e:
                print(f"❌ Error: {e}")
            print("\n")
        print("🎉 Agent demo complete!\n")
    except Exception as e:
        print("⚠️ Agent initialization or invocation failed:", e)


if __name__ == "__main__":
    main()
