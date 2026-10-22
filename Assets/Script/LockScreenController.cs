using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class LockScreenController : MonoBehaviour, IPointerClickHandler
{
    [Header("Click Catcher Image (항상 활성화)")]
    public Image clickCatcherImage;      

    [Header("Lock Screen Parent")]
    public GameObject lockScreenParent;  

    [Header("Lock Screen UI Elements")]
    public GameObject overlay;           
    public GameObject playerIcon;        
    public GameObject playerName;        
    public Button startButton;        

    [Header("Desktop UI Group")]
    public GameObject desktopUI;         

    [Header("Timings")]
    public float startDelay = 1f;        

    bool isLocked = true;

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);

        lockScreenParent.SetActive(true);
        overlay.SetActive(false);
        playerIcon.SetActive(false);
        playerName.SetActive(false);
        startButton.gameObject.SetActive(false);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (isLocked)
        {
            overlay.SetActive(true);
            playerIcon.SetActive(true);
            playerName.SetActive(true);
            startButton.gameObject.SetActive(true);

            if (clickCatcherImage != null)
                clickCatcherImage.raycastTarget = false;

            isLocked = false;
        }
    }


    void OnStartButtonClicked()
    {
        startButton.interactable = false;
        StartCoroutine(UnlockAndShowDesktop());
    }


    IEnumerator UnlockAndShowDesktop()
    {
        yield return new WaitForSeconds(startDelay);

        if (lockScreenParent != null)
            lockScreenParent.SetActive(false);

        if (desktopUI != null)
            desktopUI.SetActive(true);
    }
}
