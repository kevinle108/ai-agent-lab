
# Python LangChain - Setup

Prerequisites: Python 3.8+ installed.

## Create and activate a virtual environment

1. Create the virtual environment (run once):

```bash
cd "python-langchain"
python -m venv venv
```

2. Activate the virtual environment

- Windows (PowerShell):

```powershell
cd "python-langchain"
.\venv\Scripts\Activate.ps1
```

- Windows (Command Prompt):

```cmd
cd "python-langchain"
venv\Scripts\activate
```

- Linux / macOS:

```bash
cd python-langchain
python3 -m venv venv    # if not created yet on Unix
source venv/bin/activate
```

3. Install dependencies

```bash
pip install --upgrade pip
pip install -r requirements.txt
```

4. Deactivate when finished:

```bash
deactivate
```

---

File: `requirements.txt` contains the project dependencies.
