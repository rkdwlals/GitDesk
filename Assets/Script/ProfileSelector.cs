using UnityEngine;
using System.Collections;

public class ProfileSelector : MonoBehaviour
{
    [Tooltip("이 프로필을 클릭했을 때 활성화될 대화창")]
    public GameObject linkedChatWindow;

    [Tooltip("이 프로필의 파란색 강조 배경 오브젝트 (비활성화된 상태로 시작)")]
    public GameObject blueHighlight;

    [Tooltip("이 프로필 클릭 시 실행할 메신저 매니저 (대화 출력용)")]
    public MessengerManager messengerManager;

    private static ProfileSelector currentlySelected;
    public void OnClickFromButton()
    {
        // 이미 선택된 프로필이면 무시
        if (blueHighlight != null && blueHighlight.activeSelf)
        {
            return;
        }

        // 이전 프로필 해제
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.DeactivateHighlight();
        }

        // 현재 프로필 선택
        ActivateHighlight();
        currentlySelected = this;
    }

    private void ActivateHighlight()
    {
        if (blueHighlight != null)
            blueHighlight.SetActive(true);

        StartCoroutine(ActivateChatAfterDelay());
    }

    private IEnumerator ActivateChatAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (linkedChatWindow != null)
            linkedChatWindow.SetActive(true);

        //if (messengerManager != null)
           // messengerManager.StartIntroConversation(blueHighlight);
    }


    private void DeactivateHighlight()
    {
        if (blueHighlight != null)
            blueHighlight.SetActive(false);

        if (linkedChatWindow != null)
            linkedChatWindow.SetActive(false);
    }
}
