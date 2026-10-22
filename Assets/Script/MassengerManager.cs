using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessengerManager : MonoBehaviour
{
    [System.Serializable]
    public class MessageData
    {
        public string senderName;   // 예: "박 주임"
        public string messageText;  // 예: "처음 뵙겠습니다."
        public bool showProfile;    // 프로필 포함 여부 (true면 chatbox, false면 chatbox2)
    }

    public GameObject chatbox;
    public GameObject chatbox2;
    public Transform pageParent;
    public float chatboxStartX = -200f;
    public float chatbox2StartX = 200f;
    public float chatboxStartY = 250f;
    public float spacingY = 120f;

    public List<MessageData> introMessages = new List<MessageData>();
    public List<MessageData> currentMessages = new List<MessageData>();
    public Dictionary<int, GameObject> pageParents = new Dictionary<int, GameObject>();

    public int currentPage = 0;

    private void Start()
    {
        // 예시 메시지 삽입 및 출력 시작 가능
        StartCoroutine(SequentialChatboxCreationFromIndex(0));
    }

    public void AddMessage(string sender, string message, bool showProfile = false)
    {
        MessageData newMessage = new MessageData
        {
            senderName = sender,
            messageText = message,
            showProfile = showProfile
        };
        introMessages.Add(newMessage);
        currentMessages.Add(newMessage); // 바로 출력하도록 추가
    }

    public IEnumerator SequentialChatboxCreationFromIndex(int startIndex)
    {
        int pageIndex = currentPage;
        GameObject pageGroup = pageParents.ContainsKey(pageIndex) ? pageParents[pageIndex] : CreatePageGroup(pageIndex);
        pageGroup.SetActive(true);

        float currentY = chatboxStartY;
        if (pageGroup.transform.childCount > 0)
        {
            RectTransform lastRect = pageGroup.transform.GetChild(pageGroup.transform.childCount - 1).GetComponent<RectTransform>();
            currentY = lastRect.anchoredPosition.y - spacingY;
        }

        for (int i = startIndex; i < currentMessages.Count; i++)
        {
            var data = currentMessages[i];
            GameObject prefab = data.showProfile ? chatbox : chatbox2;
            GameObject bubble = Instantiate(prefab, pageGroup.transform);
            bubble.GetComponentInChildren<Text>().text = "";

            float x = data.showProfile ? chatboxStartX : chatbox2StartX;
            currentY -= data.showProfile ? 15f : spacingY;

            RectTransform rt = bubble.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(x, currentY);

            yield return StartCoroutine(TypeText(bubble.GetComponentInChildren<Text>(), data.messageText));
            yield return new WaitForSeconds(0.5f);
        }

        UpdatePage();
    }

    IEnumerator TypeText(Text textComponent, string message)
    {
        for (int i = 0; i <= message.Length; i++)
        {
            textComponent.text = message.Substring(0, i);
            yield return new WaitForSeconds(0.02f);
        }
    }

    GameObject CreatePageGroup(int pageIndex)
    {
        GameObject group = new GameObject($"Page_{pageIndex}", typeof(RectTransform));
        group.transform.SetParent(pageParent, false);
        pageParents.Add(pageIndex, group);
        return group;
    }

    void UpdatePage()
    {
        foreach (var kvp in pageParents)
        {
            kvp.Value.SetActive(kvp.Key == currentPage);
        }
    }
}
