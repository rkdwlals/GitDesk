using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CodeTestManager : MonoBehaviour
{
    public string gradingServerUrl = "http://127.0.0.1:5000/grade";

    public void SubmitCode(string userCode)
    {
        StartCoroutine(SendCodeToServer(userCode));
    }

    IEnumerator SendCodeToServer(string code)
    {
        WWWForm form = new WWWForm();
        form.AddField("code", code);

        using (UnityWebRequest www = UnityWebRequest.Post(gradingServerUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("서버 에러: " + www.error);
            }
            else
            {
                // 응답 JSON 예시: { "result": "정답입니다!", "feedback": "변수명을 더 명확히 작성해보세요." }
                string json = www.downloadHandler.text;
                ServerResponse response = JsonUtility.FromJson<ServerResponse>(json);

                OnCodeEvaluated(response.result, response.feedback);
            }
        }
    }

    void OnCodeEvaluated(string resultText, string aiFeedback)
    {
        MessengerManager messenger = FindObjectOfType<MessengerManager>();
        if (messenger != null)
        {
            int startIndex = messenger.currentMessages.Count;

            messenger.AddMessage("채점 결과", resultText, true);
            messenger.AddMessage("AI 피드백", aiFeedback, false);

            messenger.StartCoroutine(messenger.SequentialChatboxCreationFromIndex(startIndex));
        }
    }

    [System.Serializable]
    public class ServerResponse
    {
        public string result;
        public string feedback;
    }
}
