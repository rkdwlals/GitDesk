using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 이미지 아이콘(UI Image)에 붙여서 클릭 시 지정한 코딩 창을 활성화합니다.
/// </summary>
public class IconOpener : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("연결할 코딩 창 GameObject를 할당하세요.")]
    public GameObject codeWindow;

    /// <summary>
    /// 아이콘이 클릭되면 호출됩니다.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (codeWindow != null)
        {
            codeWindow.SetActive(true);
        }
        else
        {
            Debug.LogWarning("CodeWindow가 할당되지 않았습니다.", this);
        }
    }
}
