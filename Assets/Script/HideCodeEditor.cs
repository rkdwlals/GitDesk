using UnityEngine;
using UnityEngine.UI;

public class HideCodeEditor : MonoBehaviour
{
    [Tooltip("숨기고 싶은 코딩창(패널) GameObject")]
    public GameObject codeWindow;

    [Tooltip("숨기기 버튼(Button 컴포넌트)")]
    public Button hideButton;

    void Awake()
    {
        if (hideButton == null || codeWindow == null)
        {
            Debug.LogWarning("HideCodeEditor: hideButton 또는 codeWindow가 할당되지 않았습니다.");
            return;
        }

        hideButton.onClick.AddListener(HideWindow);
    }

    private void HideWindow()
    {
        codeWindow.SetActive(false);
    }
}
