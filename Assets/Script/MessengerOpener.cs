using UnityEngine;
using UnityEngine.EventSystems;
public class MessengerOpener : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("연결할 메신저 창 GameObject를 할당하세요.")]
    public GameObject messengerWindow;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (messengerWindow != null)
        {
            messengerWindow.SetActive(true);
        }
        else
        {
            Debug.LogWarning("MessengerWindow가 할당되지 않았습니다.", this);
        }
    }
}
