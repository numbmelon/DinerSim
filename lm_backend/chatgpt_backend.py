from flask import Flask, request, jsonify
from openai import OpenAI

api_key = "YOUR_API_KEY"

client = OpenAI(api_key=api_key)
app = Flask(__name__)


@app.route('/gpt', methods=['POST'])
def generate_text():
    prompt = request.json.get('prompt')
    message = client.chat.completions.create(
        model="gpt-4o-2024-05-13",
        messages=[
            {"role": "user", "content": prompt},    
        ],
        temperature=0
    )
    print(message.choices[0].message.content)
    
    return jsonify({'content': message.choices[0].message.content, 'completion_tokens': message.usage.completion_tokens, 'prompt_tokens': message.usage.prompt_tokens, 'total_tokens': message.usage.total_tokens})

if __name__ == '__main__':
    app.run(port=5012, debug=True)
    