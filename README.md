# GitDesk

개발자 인턴이 되어 회사 메신저로 받은 파이썬 문제를 푸는 **코딩 학습 시뮬레이션 게임**입니다.
게임 안에서 작성한 코드를 Flask 서버로 보내 실행·채점하고, GPT 피드백을 상사의 메신저 메시지로 보여 줍니다.

- 2025년 6월, 순천향대학교 '메타버스게임개발실습' 2인 팀 프로젝트
- 한국디지털콘텐츠학회 2025 하계종합학술대회 포스터 발표, **동상** 수상
- 플레이 영상: [포트폴리오에서 보기](https://rkdwlals.github.io/#gitdesk)

## 맡은 역할

| 담당 | 내용 |
|---|---|
| 강지민 | GPT 연결, Flask 채점 서버, 정답(채점) 시스템 등 개발 |
| 팀원 | 메신저 화면, 컴퓨터 화면 전환 그림, 상점 |

`Assets/Layout`의 그림 중 메신저 화면과 컴퓨터 화면 전환 그림은 팀원 작업입니다.

## 기술

Unity 2022.3.45f1 · C# · Python(Flask) · OpenAI API

## 폴더 구성

```
Assets/
  Scenes/Main.unity        게임 씬
  Script/                  게임 스크립트
  Layout/                  화면 그림
  Fonts/                   폰트
Server/
  server.py                채점 서버 (Flask + GPT 피드백)
  requirements.txt
```

### 주요 스크립트

| 파일 | 하는 일 |
|---|---|
| `CodeEditor.cs` | 게임 속 코드 편집기. 실행 취소/다시 실행, 복사·잘라내기·붙여넣기, 전체 선택, 자동 들여쓰기, 괄호 자동 닫기 |
| `CodeTestManager.cs` | 작성한 코드를 채점 서버로 보내고, 채점 결과와 AI 피드백을 메신저 메시지로 추가 |
| `MassengerManager.cs` | 메신저 메시지 목록 관리, 말풍선을 차례로 만들고 글자를 한 자씩 출력, 페이지 넘김 |
| `MonitorZoomController.cs` | 모니터 확대/축소, 로딩 화면 뒤 잠금 화면 표시 |
| `LockScreenController.cs` | 잠금 화면 클릭 시 프로필과 시작 버튼 표시 |
| `ProfileSelector.cs` | 메신저 프로필 선택 시 강조 표시 후 해당 대화창 열기 |
| `DropdownMenuController.cs` | 코드 편집기의 File / Edit 드롭다운 메뉴 |
| `IconOpener.cs`, `MessengerOpener.cs`, `HideCodeEditor.cs` | 바탕화면 아이콘으로 창 열기·닫기 |

## 소스 버전에 대한 안내

- 이 저장소의 Unity 프로젝트는 **2025년 6월 1일 버전**입니다. 발표 시연(6월 2일) 버전에 있던 상점·설정 화면은 이 소스에 들어 있지 않습니다. 완성된 플레이는 위 플레이 영상을 참고해 주세요.
- `Server/server.py`는 Unity 프로젝트와 다른 시점의 코드입니다. Unity의 `CodeTestManager`는 `/grade` 주소로 `code`만 보내고 `result`·`feedback`을 받도록 되어 있지만, `server.py`는 `/check` 주소로 `code`·`test_case_input`·`expected_output`을 받고 `result`·`ai_feedback`을 돌려줍니다. 그대로는 서로 연결되지 않습니다.

## 서버 실행

```bash
pip install -r Server/requirements.txt
# Windows: set OPENAI_API_KEY=발급받은_키
export OPENAI_API_KEY=발급받은_키
python Server/server.py   # http://127.0.0.1:5000
```

서버는 받은 코드를 그대로 실행하므로 **내 컴퓨터에서 테스트 용도로만** 실행하세요.

## 포함하지 않은 외부 에셋

라이선스 때문에 아래 에셋은 저장소에 넣지 않았습니다. 프로젝트를 열면 이 에셋을 쓰는 부분은 그림이 빠져 보입니다.

- 2D Atlas Speechbubbles Alphabet Numbers
- 2D Pixel Room Bedroom
- PixelFJVerdana12pt 폰트

---

이 저장소의 README·.gitignore 정리와 서버 코드의 API 키 제거는 Claude(AI)의 도움을 받았습니다. 게임 코드는 팀이 작성한 그대로입니다.
