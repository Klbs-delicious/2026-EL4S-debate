from flask import Flask, request, jsonify
from flask_cors import CORS
from dotenv import load_dotenv
import requests

load_dotenv()

app = Flask(__name__)
CORS(app)

GEMINI_API_KEY = "AIzaSyAztDywA7adn1p8nJjmNzR9wmPHUKETZMI"

GEMINI_MODEL = "gemini-3.1-flash-lite"
GEMINI_URL = (
    f"https://generativelanguage.googleapis.com/v1beta/models/"
    f"{GEMINI_MODEL}:generateContent?key={GEMINI_API_KEY}"
)


@app.route("/", methods=["GET"])
def index():
    return "Flask server is running."


@app.route("/api/gemini", methods=["POST"])
def proxy_gemini():
    if not GEMINI_API_KEY:
        return jsonify({
            "error": "GEMINI_API_KEY is not set."
        }), 500

    unity_json = request.get_json()

    if unity_json is None:
        return jsonify({
            "error": "Invalid JSON."
        }), 400

    response = requests.post(
        GEMINI_URL,
        json=unity_json,
        headers={
            "Content-Type": "application/json"
        },
        timeout=60
    )

    try:
        return jsonify(response.json()), response.status_code
    except Exception:
        return response.text, response.status_code


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)