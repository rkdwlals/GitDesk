from flask import Flask, request, jsonify
import subprocess
import os
import openai

app = Flask(__name__)
openai.api_key = os.environ.get("OPENAI_API_KEY")  # 키는 환경 변수로 설정

@app.route("/check", methods=["POST"])
def check():
    user_code = request.form["code"]
    test_input = request.form["test_case_input"]
    expected_output = request.form["expected_output"]

    # 실행용 코드 생성 (입력 삽입 포함)
    with open("user_code.py", "w", encoding="utf-8") as f:
        f.write(f"input = {test_input}\n")
        f.write(user_code)

    try:
        result = subprocess.check_output(["python", "user_code.py"], stderr=subprocess.STDOUT, timeout=5)
        output = result.decode().strip()
    except subprocess.CalledProcessError as e:
        output = f"실행 오류: {e.output.decode()}"
    except subprocess.TimeoutExpired:
        output = "실행 시간 초과"

    if output == expected_output:
        judgement = "이번 신입이 에이스네 잘했어요"
    else:
        judgement = f"신입이라 그런 지 실수했나보네. (출력값:{output})"

    ai_feedback = analyze_code_with_ai(user_code)

    return jsonify({"result": judgement, "ai_feedback": ai_feedback})


def analyze_code_with_ai(code):
    prompt = f"아래 파이썬 코드에 대한 문제점이나 개선점을 알려줘.\n\n{code}"
    try:
        response = openai.ChatCompletion.create(
            model="gpt-3.5-turbo",
            messages=[{"role": "user", "content": prompt}]
        )
        return response.choices[0].message.content.strip()
    except Exception as e:
        return f"AI 분석 실패: {str(e)}"

if __name__ == "__main__":
    app.run(debug=False)
