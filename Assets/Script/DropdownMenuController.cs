using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropdownMenuController : MonoBehaviour
{
    [Header("Button & Panel ¿¬°á")]
    public Button fileButton;
    public GameObject fileDropdown;
    public Button editButton;
    public GameObject editDropdown;

    void Start()
    {
        fileDropdown.SetActive(false);
        editDropdown.SetActive(false);

        fileButton.onClick.AddListener(() => ToggleDropdown(fileDropdown));
        editButton.onClick.AddListener(() => ToggleDropdown(editDropdown));
    }

    void ToggleDropdown(GameObject dropdown)
    {
        bool wasOn = dropdown.activeSelf;
        CloseAll();
        dropdown.SetActive(!wasOn);
    }

    void CloseAll()
    {
        fileDropdown.SetActive(false);
        editDropdown.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (fileDropdown.activeSelf || editDropdown.activeSelf)
            {
                GameObject clicked = EventSystem.current.currentSelectedGameObject;

                if (!IsPointerOverUI(fileDropdown) &&
                    !IsPointerOverUI(editDropdown) &&
                    clicked != fileButton.gameObject &&
                    clicked != editButton.gameObject)
                {
                    CloseAll();
                }
            }
        }
    }

    bool IsPointerOverUI(GameObject uiObject)
    {
        PointerEventData pd = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pd, results);
        foreach (var r in results)
        {
            if (r.gameObject.transform.IsChildOf(uiObject.transform))
                return true;
        }
        return false;
    }
}
