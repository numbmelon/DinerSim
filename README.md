# DinerSim

**Online Demos and Documents on generating data, running experiments and extending this environment will continue to be added!**

unity version: windows, 2021.3.26f1c1

## How to use
### Launch LLM Backend
An example file is provided at [chatgpt_backend.py](https://github.com/numbmelon/DinerSim/tree/main/lm_backend/chatgpt_backend.py) to launch a ChatGPT backend using OpenAI API key.
#### 1. Install backend requirements
```bash
pip install flask
pip install openai
```

#### 2. Set Your API Key
Ensure you have set your OpenAI API key in the appropriate environment variable or configuration file. In `chatgpt_backend.py`, modify the following line of code:
```python
api_key = "YOUR_API_KEY"
```

#### 3. Launch LLM backend
```bash
cd lm_backend
python chatgpt_backend.py
```

### Open the Unity Project
Open our executable file to start various tasks!
