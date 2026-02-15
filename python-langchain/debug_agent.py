from langchain.agents import create_agent
from langchain_core.tools import Tool
from langchain_openai import ChatOpenAI
from dotenv import load_dotenv
import os

load_dotenv()
token = os.getenv("GITHUB_TOKEN")

chat = ChatOpenAI(
    model="openai/gpt-4o",
    temperature=0,
    base_url="https://models.github.ai/inference",
    api_key=token,
)

def calculator(expression: str) -> str:
    try:
        result = eval(expression, {"__builtins__": None}, {})
        return str(result)
    except Exception as e:
        return f"Error: {e}"

tools = [
    Tool(
        name="Calculator",
        func=calculator,
        description="Evaluate mathematical expressions.",
    )
]

agent = create_agent(chat, tools=tools, debug=True)
print(f"Agent type: {type(agent)}")
print(f"Agent attributes: {[x for x in dir(agent) if not x.startswith('_')]}")

# Try to understand the agent structure
if hasattr(agent, 'graph'):
    print("Agent has 'graph' attribute")
if hasattr(agent, 'invoke'):
    print("Agent has 'invoke' method")
if hasattr(agent, 'input_schema'):
    print(f"Agent input schema: {agent.input_schema}")

# Try invoking
try:
    result = agent.invoke({"messages": [("user", "What is 25 * 4 + 10?")]})
    print(f"Result: {result}")
except Exception as e:
    print(f"Invoke failed: {e}")
    import traceback
    traceback.print_exc()
