using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CodeEditor : MonoBehaviour
{
    // ----------------------------------
    // 에디터 UI 컴포넌트
    // ----------------------------------
    [Header("UI Components")]
    public InputField inputField;       // UnityEngine.UI InputField (legacy)
    public UnityEngine.UI.Button testButton;
    // 서버 전송용 Test 버튼
    public CodeTestManager codeTestManager; // 서버 통신 담당 매니저

    // ----------------------------------
    // Undo/Redo 상태 저장용 스택
    // ----------------------------------
    private Stack<State> undoStack = new Stack<State>();
    private Stack<State> redoStack = new Stack<State>();
    private bool isProcessingUndoRedo = false;  // Undo/Redo 처리 중인지 플래그
    private bool isProcessingInput = false;     // 입력 처리 중인지 플래그

    // ----------------------------------
    // 탭/들여쓰기 설정
    // ----------------------------------
    private string indentString = "    ";       // 탭 대신 사용할 공백 4칸

    /// <summary>
    /// Awake: 초기 리스너 등록 및 초기 상태 저장
    /// </summary>
    private void Awake()
    {
        // 멀티라인 및 줄바꿈 허용
        inputField.lineType = InputField.LineType.MultiLineNewline;
        inputField.textComponent.supportRichText = true;

        // 텍스트 변경 이벤트
        inputField.onValueChanged.AddListener(OnTextChanged);
        // 개별 문자 입력 유효성 검사 (탭/엔터/괄호 자동 처리)
        inputField.onValidateInput += ValidateInput;
        // Test 버튼 클릭 이벤트
        testButton.onClick.AddListener(OnTestCode);

        // 초기 에디터 상태(빈 코드) 저장
        undoStack.Push(new State { text = inputField.text, caretPos = inputField.caretPosition });
    }

    /// <summary>
    /// 텍스트가 변경될 때 호출: Undo 스택에 상태 저장, Redo 스택 초기화
    /// </summary>
    private void OnTextChanged(string newText)
    {
        if (isProcessingUndoRedo) return;
        undoStack.Push(new State { text = inputField.text, caretPos = inputField.caretPosition });
        redoStack.Clear();
    }

    /// <summary>
    /// 개별 문자 입력 시 동작 제어: 탭, 엔터, 쌍 괄호 자동 완성 등
    /// </summary>
    private char ValidateInput(string text, int charIndex, char addedChar)
    {
        if (isProcessingInput) return addedChar;

        // Tab 키 누를 때
        if (addedChar == '\t')
        {
            isProcessingInput = true;
            InsertTab(inputField.caretPosition);
            isProcessingInput = false;
            return '\0';   // 기본 탭 처리는 무시
        }

        // Enter 키 누를 때
        if (addedChar == '\n')
        {
            isProcessingInput = true;
            InsertNewlineAndIndent(inputField.caretPosition);
            isProcessingInput = false;
            return '\0';   // 기본 줄바꿈 처리 무시
        }

        // 여는 괄호/따옴표 자동 쌍 삽입
        if ("({[\"']".IndexOf(addedChar) >= 0)
        {
            isProcessingInput = true;
            InsertPair(addedChar);
            isProcessingInput = false;
            return '\0';
        }

        // 닫는 괄호 스킵
        if (")}]\"'".IndexOf(addedChar) >= 0)
        {
            isProcessingInput = true;
            bool skipped = SkipClosing(addedChar);
            isProcessingInput = false;
            return skipped ? '\0' : addedChar;
        }

        return addedChar; // 그 외 기본 처리
    }

    /// <summary>
    /// OnGUI: 키보드 단축키 처리 (Undo/Redo, 스마트 백스페이스)
    /// </summary>
    void OnGUI()
    {
        if (!inputField.isFocused) return;
        Event e = Event.current;
        bool ctrl = e.control || e.command;

        // Ctrl+Z: Undo
        if (ctrl && e.keyCode == KeyCode.Z && e.type == EventType.KeyDown && !e.shift)
        {
            PerformUndo();
            e.Use();
            return;
        }
        // Ctrl+Y or Ctrl+Shift+Z: Redo
        if (ctrl && ((e.shift && e.keyCode == KeyCode.Z) || e.keyCode == KeyCode.Y)
                    && e.type == EventType.KeyDown)
        {
            PerformRedo();
            e.Use();
            return;
        }
        // Backspace: 스마트 인덴트 삭제
        if (e.keyCode == KeyCode.Backspace && e.type == EventType.KeyDown)
        {
            isProcessingInput = true;
            DeleteSmartIndent();
            isProcessingInput = false;
            e.Use();
            return;
        }
    }

    /// <summary>
    /// Undo 실행: 이전 상태로 복원
    /// </summary>
    private void PerformUndo()
    {
        if (undoStack.Count <= 1) return;
        isProcessingUndoRedo = true;

        State current = undoStack.Pop();
        redoStack.Push(current);
        State prev = undoStack.Peek();

        inputField.text = prev.text;
        inputField.caretPosition =
            inputField.selectionAnchorPosition =
            inputField.selectionFocusPosition = prev.caretPos;

        isProcessingUndoRedo = false;
    }

    /// <summary>
    /// Redo 실행: Redo 스택에서 상태 복원
    /// </summary>
    public void PerformRedo()
    {
        if (redoStack.Count == 0) return;
        isProcessingUndoRedo = true;

        State next = redoStack.Pop();
        undoStack.Push(next);
        inputField.text = next.text;
        inputField.caretPosition =
            inputField.selectionAnchorPosition =
            inputField.selectionFocusPosition = next.caretPos;

        isProcessingUndoRedo = false;
    }

    // ----------------------------------
    // 클립보드 & 선택 기능
    // ----------------------------------
    public void Copy()
    {
        int s = Mathf.Min(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);
        int e = Mathf.Max(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);
        if (s == e) return;
        GUIUtility.systemCopyBuffer = inputField.text.Substring(s, e - s);
    }

    public void DeleteSelection()
    {
        int s = Mathf.Min(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);
        int e = Mathf.Max(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);
        if (s == e) return;
        inputField.text = inputField.text.Remove(s, e - s);
        inputField.caretPosition =
            inputField.selectionAnchorPosition =
            inputField.selectionFocusPosition = s;
    }

    public void Cut() { Copy(); DeleteSelection(); }

    public void Paste()
    {
        string clip = GUIUtility.systemCopyBuffer;
        int pos = inputField.caretPosition;
        inputField.text = inputField.text.Insert(pos, clip);
        int np = pos + clip.Length;
        inputField.caretPosition =
            inputField.selectionAnchorPosition =
            inputField.selectionFocusPosition = np;
    }

    public void SelectAll()
    {
        inputField.selectionAnchorPosition = 0;
        inputField.selectionFocusPosition = inputField.text.Length;
    }

    // ----------------------------------
    // 탭 및 스마트 인덴트 처리
    // ----------------------------------
    private void InsertTab(int pos)
    {
        string txt = inputField.text;
        pos = Mathf.Clamp(pos, 0, txt.Length);

        int searchPos = Mathf.Clamp(pos - 1, 0, txt.Length - 1);
        int lineStart = txt.LastIndexOf('\n', searchPos);
        lineStart = (lineStart >= 0) ? lineStart + 1 : 0;

        int spaceCount = 0;
        for (int i = lineStart; i < pos; i++)
            if (txt[i] == ' ') spaceCount++;
            else break;

        int relPos = pos - lineStart;
        int toAdd = (relPos <= spaceCount)
            ? (relPos % indentString.Length == 0 ? indentString.Length : indentString.Length - (relPos % indentString.Length))
            : indentString.Length;

        string spaces = new string(' ', toAdd);
        inputField.text = txt.Insert(pos, spaces);
        int newPos = pos + toAdd;
        inputField.caretPosition =
            inputField.selectionAnchorPosition =
            inputField.selectionFocusPosition = newPos;
    }

    private void DeleteSmartIndent()
    {
        int pos = inputField.caretPosition;
        string txt = inputField.text;
        if (pos >= indentString.Length && txt.Substring(pos - indentString.Length, indentString.Length) == indentString)
        {
            inputField.text = txt.Remove(pos - indentString.Length, indentString.Length);
            inputField.caretPosition = pos - indentString.Length;
            inputField.selectionAnchorPosition = inputField.selectionFocusPosition = inputField.caretPosition;
        }
        else if (pos > 0)
        {
            inputField.text = txt.Remove(pos - 1, 1);
            inputField.caretPosition = pos - 1;
            inputField.selectionAnchorPosition = inputField.selectionFocusPosition = inputField.caretPosition;
        }
    }

    private void InsertNewlineAndIndent(int pos)
    {
        // 줄바꿈과 기존 들여쓰기 유지, '{' 다음 줄엔 추가 들여쓰기
        string txt = inputField.text;
        pos = Mathf.Clamp(pos, 0, txt.Length);

        // 닫는 중괄호 특별 처리: 한 줄 들여쓰기 후 줄바꿈
        if (pos < txt.Length && txt[pos] == '}')
        {
            int lastNl = txt.LastIndexOf('\n', pos - 1);
            int lineStart = lastNl >= 0 ? lastNl + 1 : 0;
            string line = txt.Substring(lineStart, pos - lineStart);
            string indentPrev = Regex.Match(line, "^[ \t]*").Value;

            string inner = indentPrev + indentString;
            string closing = indentPrev;
            string toInsert = "\n" + inner + "\n" + closing;

            inputField.text = txt.Insert(pos, toInsert);
            inputField.caretPosition = pos + 1 + inner.Length;
            inputField.selectionAnchorPosition = inputField.selectionFocusPosition = inputField.caretPosition;
            return;
        }

        // 기본 줄바꿈: 기존 들여쓰기 복사
        int lastNl2 = txt.LastIndexOf('\n', pos - 1);
        int start = lastNl2 >= 0 ? lastNl2 + 1 : 0;
        string currentLine = txt.Substring(start, pos - start);
        string basicIndent = Regex.Match(currentLine, "^[ \t]*").Value;
        bool extraIndent = currentLine.TrimEnd().EndsWith("{");
        string toInsert2 = "\n" + basicIndent + (extraIndent ? indentString : "");

        inputField.text = txt.Insert(pos, toInsert2);
        inputField.caretPosition = pos + toInsert2.Length;
        inputField.selectionAnchorPosition = inputField.selectionFocusPosition = inputField.caretPosition;
    }

    // ----------------------------------
    // 자동 쌍 삽입 / 스킵
    // ----------------------------------
    private void InsertPair(char open)
    {
        string pair = open == '(' ? "()" :
                      open == '{' ? "{}" :
                      open == '[' ? "[]" :
                      open == '"' ? "\"\"" :
                      open == '\'' ? "''" : null;
        if (pair == null) return;

        string txt = inputField.text;
        int a = inputField.selectionAnchorPosition;
        int f = inputField.selectionFocusPosition;
        int s = Mathf.Clamp(Mathf.Min(a, f), 0, txt.Length);
        int e = Mathf.Clamp(Mathf.Max(a, f), 0, txt.Length);

        if (s != e) // 영역 선택 시 감싸기
        {
            string sel = txt.Substring(s, e - s);
            string wrapped = open + sel + pair[1];
            inputField.text = txt.Remove(s, e - s).Insert(s, wrapped);
            int newPos = s + wrapped.Length;
            inputField.caretPosition =
                inputField.selectionAnchorPosition =
                inputField.selectionFocusPosition = newPos;
        }
        else // 단일 삽입
        {
            inputField.text = txt.Insert(s, pair);
            inputField.caretPosition = s + 1;
            inputField.selectionAnchorPosition = inputField.selectionFocusPosition = inputField.caretPosition;
        }
    }

    private bool SkipClosing(char close)
    {
        // 이미 닫는 괄호가 있으면 커서만 이동
        string txt = inputField.text;
        int pos = inputField.caretPosition;
        if (pos < txt.Length && txt[pos] == close)
        {
            inputField.caretPosition = pos + 1;
            inputField.selectionAnchorPosition = inputField.selectionFocusPosition = inputField.caretPosition;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Test 버튼 클릭 시 호출: 코드 내용을 CodeTestManager로 전달
    /// </summary>
    private void OnTestCode()
    {
        string code = inputField.text;
        codeTestManager.SubmitCode(code);

    }

    // ----------------------------------
    // 내부 상태 저장용 클래스
    // ----------------------------------
    private class State
    {
        public string text;
        public int caretPos;
    }
}